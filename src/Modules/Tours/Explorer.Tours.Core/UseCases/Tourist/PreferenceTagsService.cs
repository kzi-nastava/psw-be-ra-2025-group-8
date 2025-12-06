using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.API.Public.Tourist;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Tours.Core.UseCases.Tourist
{
    public class PreferenceTagsService : IPreferenceTagsService
    {
        private readonly ITouristPreferencesRepository _touristPreferencesRepository;
        private readonly ITagsRepository _tagsRepository;
        private readonly IPreferenceTagsRepository _preferenceTagsRepository;
        private readonly IMapper _mapper;

        public PreferenceTagsService(
            ITouristPreferencesRepository touristPreferencesRepository,
            ITagsRepository tagsRepository,
            IPreferenceTagsRepository preferenceTagsRepository,
            IMapper mapper)
        {
            _touristPreferencesRepository = touristPreferencesRepository;
            _tagsRepository = tagsRepository;
            _preferenceTagsRepository = preferenceTagsRepository;
            _mapper = mapper;
        }

        public IEnumerable<TagDto> GetTagsForPerson(long personId)
        {
            var tags = _preferenceTagsRepository.GetTagsForPerson(personId);
            return tags.Select(t => new TagDto { Id = t.Id, Tag = t.Tag }).ToList();
        }

        public TagDto AddTagForPerson(long personId, TagDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Tag))
                throw new ArgumentException("Tag cannot be empty");

            var tagTrim = dto.Tag.Trim();
            if (tagTrim.Length > 20)
                throw new ArgumentException("Tag cannot be longer than 20 characters");

            // get tourist preferences for that person
            var touristPreferences = _touristPreferencesRepository.GetByPersonId(personId);
            if (touristPreferences == null)
            {
                return null;
            }

            var normalized = tagTrim.ToLowerInvariant();

            // da li tag postoji globalno
            var existingTag = _tagsRepository.GetByName(normalized);
            if (existingTag == null)
            {
                // ako ne kreiraj novi tag
                existingTag = _tagsRepository.Create(new Tags(tagTrim));
            }

            // korisnik ne sme imati vec ovaj tag
            if (_preferenceTagsRepository.Exists(touristPreferences.Id, existingTag.Id))
            {
                return new TagDto { Id = existingTag.Id, Tag = existingTag.Tag };
            }

            var pt = new PreferenceTags(touristPreferences.Id, existingTag.Id);
            _preferenceTagsRepository.Add(pt);

            return new TagDto { Id = existingTag.Id, Tag = existingTag.Tag };
        }

        public void RemoveTagFromPerson(long personId, long tagId)
        {
            var touristPreferences = _touristPreferencesRepository.GetByPersonId(personId);
            if (touristPreferences == null)
                return;

            _preferenceTagsRepository.Delete(touristPreferences.Id, tagId);
        }
    }
}
