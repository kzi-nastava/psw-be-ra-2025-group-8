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
    public class PreferencesService : IPreferencesService
    {
        private readonly ICrudRepository<Preferences> _repository;
        private readonly IMapper _mapper;

        public PreferencesService(
            ICrudRepository<Preferences> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public PreferencesDto Get(long personId)
        {
            var entity = _repository.Get(personId);
            return _mapper.Map<PreferencesDto>(entity);
        }

        public PreferencesDto Create(long personId, PreferencesDto dto)
        {
            var entity = _mapper.Map<Preferences>(dto);
            //entity.Set("PersonId", personId);
            entity.PersonId = personId;

            var created = _repository.Create(entity);
            return _mapper.Map<PreferencesDto>(created);
        }

        public PreferencesDto Update(long personId, PreferencesDto dto)
        {
            var existing = _repository.Get(personId);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            //existing.Set("PersonId", personId);
            existing.PersonId = personId;

            var updated = _repository.Update(existing);
            return _mapper.Map<PreferencesDto>(updated);
        }

        public void Delete(long personId)
        {
            _repository.Delete(personId);
        }
    }
}
