using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Tours.Core.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.BuildingBlocks.Core.Exceptions;



namespace Explorer.Tours.Core.UseCases.Tourist
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
            //novo dodato
            if (entity == null)
            {
                entity = _repository.Create(
                    new TouristPreferences(personId, DifficultyLevel.Beginner));
            }
            return _mapper.Map<TouristPreferencesDto>(entity);
        }


        public TouristPreferencesDto Update(long personId, UpdateTouristPreferencesDto dto)
        {
            var existing = _repository.GetByPersonId(personId);
            if (existing == null)
            {
                // Ako iz nekog razloga neko šalje PUT pre GET-a:
                existing = _repository.Create(
                    new TouristPreferences(personId, DifficultyLevel.Beginner));
            }

            // dto.Difficulty je string, mapi ga na enum:
            if (!Enum.TryParse<DifficultyLevel>(dto.Difficulty, out var diff))
            {
                throw new EntityValidationException($"Invalid difficulty '{dto.Difficulty}'.");
            }

            existing.Difficulty = diff;
            existing.PersonId = personId;

            var updated = _repository.Update(existing);
            return _mapper.Map<TouristPreferencesDto>(updated);
        }
    }
}

