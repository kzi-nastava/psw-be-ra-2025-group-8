using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class TourRatingImageRepository : ITourRatingImageRepository
    {
        private readonly ToursContext _context;

        public TourRatingImageRepository(ToursContext context)
        {
            _context = context;
        }

        public TourRatingImage Create(TourRatingImage image)
        {
            // Ensure Id is 0 so EF Core will generate it
            var entry = _context.Entry(image);
            entry.State = Microsoft.EntityFrameworkCore.EntityState.Added;
            
            _context.SaveChanges();
            return image;
        }

        public List<TourRatingImage> GetByTourRatingId(long tourRatingId)
        {
            return _context.TourRatingImages
                .Where(img => img.TourRatingId == tourRatingId)
                .OrderBy(img => img.UploadedAt)
                .ToList();
        }

        public TourRatingImage Get(long id)
        {
            return _context.TourRatingImages.Find(id);
        }

        public void Delete(long id)
        {
            var image = _context.TourRatingImages.Find(id);
            if (image != null)
            {
                _context.TourRatingImages.Remove(image);
                _context.SaveChanges();
            }
        }
    }
}
