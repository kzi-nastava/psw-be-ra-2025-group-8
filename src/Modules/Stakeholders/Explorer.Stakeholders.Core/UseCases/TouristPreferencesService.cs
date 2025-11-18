using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;



namespace Explorer.Stakeholders.Core.UseCases
{
    public class TouristPreferencesService : ITouristPreferencesService
    {
        private readonly ITouristPreferencesRepository _repository;
        private readonly IMapper _mapper;

        public TouristPreferencesService(
            ITouristPreferencesRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public TouristPreferencesDto Get(long personId)
        {
            var entity = _repository.GetByPersonId(personId);
            return _mapper.Map<TouristPreferencesDto>(entity);
        }


        public TouristPreferencesDto Update(long personId, TouristPreferencesDto dto)
        {
            var existing = _repository.GetByPersonId(personId);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            //existing.Set("PersonId", personId);
            existing.PersonId = personId;

            var updated = _repository.Update(existing);
            return _mapper.Map<TouristPreferencesDto>(updated);
        }
    }
}

