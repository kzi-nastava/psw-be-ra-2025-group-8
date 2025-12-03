using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class TagsRepository : ITagsRepository
    {
        private readonly ToursContext _db;

        public TagsRepository(ToursContext db)
        {
            _db = db;
        }

        public Tags GetById(long id)
        {
            return _db.Tags.FirstOrDefault(t => t.Id == id);
        }

        public Tags GetByName(string tagNormalized)
        {
            return _db.Tags
                .FirstOrDefault(t => t.Tag != null &&
                                     t.Tag.ToLower().Trim() == tagNormalized);
        }

        public Tags Create(Tags tag)
        {
            var e = _db.Tags.Add(tag).Entity;
            _db.SaveChanges();
            return e;
        }
    }
}
