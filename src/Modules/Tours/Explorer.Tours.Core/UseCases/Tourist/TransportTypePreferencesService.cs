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
            // 1) Učitaj ili napravi TouristPreferences
            var pref = _touristPreferencesRepository.GetByPersonId(personId);
            if (pref == null)
            {
                // ako nema preference, napravimo je sa Beginner
                pref = _touristPreferencesRepository.Create(
                    new TouristPreferences(personId, DifficultyLevel.Beginner));
            }

            // 2) Učitaj postojeće transport preference
            var transports = _transportRepository.GetByPreferenceId(pref.Id).ToList();

            // 3) Ako nema 4 reda (Walk, Bicycle, Car, Boat), dopunimo
            var allTypes = Enum.GetValues(typeof(TransportType))
                               .Cast<TransportType>()
                               .ToList();

            var missingTypes = allTypes
                .Where(t => transports.All(x => x.Transport != t))
                .ToList();

            if (missingTypes.Any())
            {
                var newItems = missingTypes
                    .Select(t => new TransportTypePreferences(pref.Id, t, 0))
                    .ToList();

                _transportRepository.CreateRange(newItems);
                transports.AddRange(newItems);
            }

            // 4) Mapper radi svoje – ne diramo ga :)
            var ordered = transports.OrderBy(t => t.Transport).ToList();
            return _mapper.Map<IEnumerable<TransportTypePreferenceDto>>(ordered);
        }

        public void Update(long personId, IEnumerable<TransportTypePreferenceDto> dtos)
        {
            // 1) Učitaj ili napravi TouristPreferences
            var pref = _touristPreferencesRepository.GetByPersonId(personId);
            if (pref == null)
            {
                pref = _touristPreferencesRepository.Create(
                    new TouristPreferences(personId, DifficultyLevel.Beginner));
            }

            // 2) Učitaj postojeće transport preference
            var existing = _transportRepository.GetByPreferenceId(pref.Id).ToList();

            // 3) Ako nema 4 reda, dopunimo
            var allTypes = Enum.GetValues(typeof(TransportType))
                               .Cast<TransportType>()
                               .ToList();

            var missingTypes = allTypes
                .Where(t => existing.All(x => x.Transport != t))
                .ToList();

            if (missingTypes.Any())
            {
                var newItems = missingTypes
                    .Select(t => new TransportTypePreferences(pref.Id, t, 0))
                    .ToList();

                _transportRepository.CreateRange(newItems);
                existing.AddRange(newItems);
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
                    // Teoretski ne bi trebalo da se desi jer smo gore dopunili,
                    // ali ako se desi – ovo je jasnija poruka nego 500 internal.
                    throw new EntityValidationException(
                        $"Transport '{dto.Transport}' does not exist for this preference and cannot be updated.");
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
