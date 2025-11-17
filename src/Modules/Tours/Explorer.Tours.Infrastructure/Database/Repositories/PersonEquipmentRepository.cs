using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class PersonEquipmentRepository : IPersonEquipmentRepository
    {
        private readonly ToursContext _context;
        private readonly DbSet<PersonEquipment> _dbSet;

        public PersonEquipmentRepository(ToursContext context)
        {
            _context = context;
            _dbSet = context.PersonEquipment;
        }

        public IEnumerable<PersonEquipment> GetByPersonId(long personId)
        {
            return _dbSet
                .Where(pe => pe.PersonId == personId)
                .ToList();
        }

        public PersonEquipment? Find(long personId, long equipmentId)
        {
            return _dbSet
                .FirstOrDefault(pe =>
                    pe.PersonId == personId &&
                    pe.EquipmentId == equipmentId);
        }

        public void Add(PersonEquipment entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
        }

        public void Remove(PersonEquipment entity)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }
    }
}
