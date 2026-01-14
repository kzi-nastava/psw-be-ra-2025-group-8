using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class BundleRepository : IBundleRepository
{
    private readonly ToursContext _context;

    public BundleRepository(ToursContext context) => _context = context;

    private IQueryable<Bundle> BundlesWithIncludes()
        => _context.Set<Bundle>()
            .Include(b => b.BundleTours)
            .ThenInclude(bt => bt.Tour);

    public Bundle Get(long id)
        => BundlesWithIncludes().FirstOrDefault(b => b.Id == id);

    public List<Bundle> GetByAuthor(int authorId)
        => BundlesWithIncludes().Where(b => b.AuthorId == authorId).ToList();

    public List<Bundle> GetPublished()
        => BundlesWithIncludes().Where(b => b.Status == BundleStatus.Published).ToList();

    public Bundle Create(Bundle bundle)
    {
        _context.Set<Bundle>().Add(bundle);
        _context.SaveChanges();
        return bundle;
    }

    public Bundle Update(Bundle bundle)
    {
        _context.Set<Bundle>().Update(bundle);
        _context.SaveChanges();
        return bundle;
    }

    public void Delete(long id)
    {
        var entity = _context.Set<Bundle>().Include(b => b.BundleTours).FirstOrDefault(b => b.Id == id);
        if (entity == null) return;

        _context.Set<Bundle>().Remove(entity);
        _context.SaveChanges();
    }
}

