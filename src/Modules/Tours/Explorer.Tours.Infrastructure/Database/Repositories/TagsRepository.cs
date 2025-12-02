using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;

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
            return _db.Set<Tags>().Find(id);
        }

        public Tags GetByName(string tagNormalized)
        {
            // We store Tag as-is, so compare normalized
            return _db.Set<Tags>().AsEnumerable()
                .FirstOrDefault(t => t.Tag != null && t.Tag.Trim().ToLowerInvariant() == tagNormalized);
        }

        public Tags Create(Tags tag)
        {
            var e = _db.Set<Tags>().Add(tag).Entity;
            _db.SaveChanges();
            return e;
        }
    }
}
