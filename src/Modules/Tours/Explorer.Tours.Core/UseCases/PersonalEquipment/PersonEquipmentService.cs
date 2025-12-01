using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.PersonalEquipment;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.UseCases.PersonalEquipment
{
    public class PersonEquipmentService : IPersonEquipmentService
    {
        private readonly IPersonEquipmentRepository _personEquipmentRepository;
        private readonly ICrudRepository<Equipment> _equipmentRepository;

        public PersonEquipmentService(
            IPersonEquipmentRepository personEquipmentRepository,
            ICrudRepository<Equipment> equipmentRepository)
        {
            _personEquipmentRepository = personEquipmentRepository;
            _equipmentRepository = equipmentRepository;
        }

        // ADD
        public void AddEquipmentToPerson(long personId, long equipmentId)
        {
            var existing = _personEquipmentRepository.Find(personId, equipmentId);
            if (existing != null) return;

            var entity = new PersonEquipment
            {
                PersonId = personId,
                EquipmentId = equipmentId,
                AssignedAt = DateTime.UtcNow
            };

            _personEquipmentRepository.Add(entity);
        }

        // REMOVE
        public void RemoveEquipmentFromPerson(long personId, long equipmentId)
        {
            var existing = _personEquipmentRepository.Find(personId, equipmentId);
            if (existing == null) return;

            _personEquipmentRepository.Remove(existing);
        }

        // PAGED with checkbox
        public PagedResult<EquipmentForPersonDto> GetPagedForPerson(long personId, int page, int pageSize)
        {
            var allEquipment = _equipmentRepository.GetPaged(page, pageSize);

            var ownedIds = _personEquipmentRepository
                .GetByPersonId(personId)
                .Select(pe => pe.EquipmentId)
                .ToList();

            var dtoList = allEquipment.Results
                .Select(e => new EquipmentForPersonDto
                {
                    EquipmentId = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    IsOwned = ownedIds.Contains(e.Id)
                })
                .ToList();

            return new PagedResult<EquipmentForPersonDto>(dtoList, allEquipment.TotalCount);
        }
    }
}
