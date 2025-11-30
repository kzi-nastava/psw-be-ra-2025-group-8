using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.API.Public.Tourist;

namespace Explorer.Tours.Core.UseCases.Tourist
{
    public class TransportTypePreferencesService : ITransportTypePreferencesService
    {
        private readonly ITouristPreferencesRepository _touristPreferencesRepository;
        private readonly ITransportTypePreferencesRepository _transportRepository;
        private readonly IMapper _mapper;

        public TransportTypePreferencesService(
            ITouristPreferencesRepository touristPreferencesRepository,
            ITransportTypePreferencesRepository transportRepository,
            IMapper mapper)
        {
            _touristPreferencesRepository = touristPreferencesRepository;
            _transportRepository = transportRepository;
            _mapper = mapper;
        }

        public IEnumerable<TransportTypePreferenceDto> Get(long personId)
        {
            var pref = _touristPreferencesRepository.GetByPersonId(personId);
            if (pref == null) throw new NotFoundException($"Tourist preferences for person {personId} not found.");

            var transports = _transportRepository.GetByPreferenceId(pref.Id);
            return _mapper.Map<IEnumerable<TransportTypePreferenceDto>>(transports);
        }

        public void Update(long personId, IEnumerable<TransportTypePreferenceDto> dtos)
        {
            var pref = _touristPreferencesRepository.GetByPersonId(personId);
            if (pref == null) throw new NotFoundException($"Tourist preferences for person {personId} not found.");

            var existing = _transportRepository.GetByPreferenceId(pref.Id).ToList();

            var enumCount = Enum.GetValues(typeof(TransportType)).Length;
            if (existing.Count != enumCount)
            {
                throw new EntityValidationException("Transport preferences in database are in invalid state (expected all transport types to exist).");
            }

            if (dtos == null || !dtos.Any()) return;

            var seen = new HashSet<TransportType>();
            var toUpdate = new List<TransportTypePreferences>();

            foreach (var dto in dtos)
            {
                if (dto == null) continue;

                if (!Enum.TryParse<TransportType>(dto.Transport, true, out var t))
                {
                    throw new EntityValidationException($"Invalid transport value: '{dto.Transport}'.");
                }

                if (dto.Rating < 0 || dto.Rating > 3)
                {
                    throw new EntityValidationException($"Rating for {dto.Transport} must be between 0 and 3.");
                }

                if (!seen.Add(t))
                {
                    throw new EntityValidationException($"Duplicate transport in request: {dto.Transport}.");
                }

                var existingItem = existing.FirstOrDefault(x => x.Transport == t);
                if (existingItem == null)
                {
                    throw new EntityValidationException($"Transport '{dto.Transport}' does not exist for this preference and cannot be added.");
                }

                existingItem.Rating = dto.Rating;
                toUpdate.Add(existingItem);
            }

            if (toUpdate.Any())
            {
                _transportRepository.UpdateRange(toUpdate);
            }
        }
    }
}
