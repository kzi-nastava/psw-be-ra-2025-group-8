using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly PaymentsContext _context;

        public SaleRepository(PaymentsContext context)
        {
            _context = context;
        }

        private IQueryable<Sale> SalesWithIncludes()
        {
            return _context.Sales.Include(s => s.SaleTours);
        }

        public Sale Create(Sale sale)
        {
            _context.Sales.Add(sale);
            _context.SaveChanges();
            return sale;
        }

        public Sale Update(Sale sale)
        {
            var existingSale = SalesWithIncludes().FirstOrDefault(s => s.Id == sale.Id);
            if (existingSale == null)
                throw new KeyNotFoundException($"Sale with ID {sale.Id} not found.");

            // Capture desired tour IDs from the incoming aggregate before we touch tracked entities
            var desiredTourIds = sale.TourIds.ToList();

            // Update scalar values only (avoid touching navigation/backing-field collections)
            existingSale.Update(sale.StartDate, sale.EndDate, sale.DiscountPercentage, desiredTourIds);
            _context.Entry(existingSale).State = EntityState.Modified;

            // Replace SaleTours by deleting existing rows and inserting new rows
            // Materialize to avoid "Collection was modified" when EF updates navigation collection tracking.
            var oldSaleTours = existingSale.SaleTours.ToList();
            _context.SaleTours.RemoveRange(oldSaleTours);

            var newSaleTours = desiredTourIds
                .Distinct()
                .Select(tourId => new SaleTour(existingSale.Id, tourId))
                .ToList();

            _context.SaleTours.AddRange(newSaleTours);

            _context.SaveChanges();

            // Reload aggregate with includes to ensure navigation/collection is populated
            return SalesWithIncludes().AsNoTracking().First(s => s.Id == sale.Id);
        }

        public void Delete(long id)
        {
            var sale = SalesWithIncludes().FirstOrDefault(s => s.Id == id);
            if (sale != null)
            {
                _context.Sales.Remove(sale);
                _context.SaveChanges();
            }
        }

        public Sale Get(long id)
        {
            return SalesWithIncludes().FirstOrDefault(s => s.Id == id);
        }

        public Sale GetByIdAndAuthor(long id, long authorId)
        {
            return SalesWithIncludes().FirstOrDefault(s => s.Id == id && s.AuthorId == authorId);
        }

        public List<Sale> GetByAuthor(long authorId)
        {
            return SalesWithIncludes().Where(s => s.AuthorId == authorId).ToList();
        }

        public List<Sale> GetActiveSales()
        {
            var now = DateTime.UtcNow;
            return SalesWithIncludes()
                .Where(s => s.StartDate <= now && s.EndDate >= now)
                .ToList();
        }

        public List<Sale> GetActiveSalesForTour(long tourId)
        {
            var now = DateTime.UtcNow;
            return SalesWithIncludes()
                .Where(s => s.StartDate <= now && s.EndDate >= now 
                    && s.SaleTours.Any(st => st.TourId == tourId))
                .ToList();
        }

        public PagedResult<Sale> GetPaged(int page, int pageSize)
        {
            var query = SalesWithIncludes();
            var totalCount = query.Count();
            var results = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<Sale>(results, totalCount);
        }
    }
}
