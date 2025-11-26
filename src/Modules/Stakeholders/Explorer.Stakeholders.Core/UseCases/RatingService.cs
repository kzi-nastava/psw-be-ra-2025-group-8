using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IMapper _mapper;

        public RatingService(IRatingRepository ratingRepository, IMapper mapper)
        {
            _ratingRepository = ratingRepository;
            _mapper = mapper;
        }

        public RatingDto Create(RatingNoIdDto ratingDto, long userId)
        {
            var rating = _mapper.Map<Rating>(ratingDto);

            rating.CreationDate = DateTime.UtcNow;
            rating.UserId = userId;
            //rating.Validate();

            var result = _ratingRepository.Create(rating);
            return _mapper.Map<RatingDto>(result);
        }

        public PagedResult<RatingDto> GetPaged(int page, int pageSize)
        {
            var result = _ratingRepository.GetPaged(page, pageSize);
            var items = result.Results.Select(_mapper.Map<RatingDto>).ToList();
            return new PagedResult<RatingDto>(items, result.TotalCount);
        }

        public RatingDto GetByUserId(long userId)
        {
            var rating = _ratingRepository.GetByUserId(userId);

            if (rating == null) return null;

            return _mapper.Map<RatingDto>(rating);
        }

        public RatingDto Get(int id)
        {
            var result = _ratingRepository.Get(id);
            return _mapper.Map<RatingDto>(result);
        }

        public RatingDto UpdateByUserId(RatingNoIdDto ratingDto, long userId)
        {
            var existingRating = _ratingRepository.GetByUserId(userId);

            if (existingRating == null)
            {
                throw new KeyNotFoundException("Ocena nije pronađena. Kreirajte je prvo.");
            }

            _mapper.Map(ratingDto, existingRating);

            existingRating.CreationDate = DateTime.UtcNow;

            var result = _ratingRepository.Update(existingRating);
            return _mapper.Map<RatingDto>(result);
        }

        public RatingDto Update(RatingDto ratingDto, long userId)
        {
            var existingRating = _ratingRepository.Get(ratingDto.Id);

            if (existingRating == null || existingRating.UserId != userId)
            {
                throw new KeyNotFoundException("Ocena nije pronađena ili niste autorizovani.");
            }

            _mapper.Map(ratingDto, existingRating);
            existingRating.CreationDate = existingRating.CreationDate;

            var result = _ratingRepository.Update(existingRating);
            return _mapper.Map<RatingDto>(result);
        }

        public void DeleteByUserId(long userId)
        {
            var existingRating = _ratingRepository.GetByUserId(userId);

            if (existingRating == null)
            {
                return;
            }

            _ratingRepository.Delete((int)existingRating.Id);
        }

        public void Delete(int id, long userId)
        {
            var existingRating = _ratingRepository.Get(id);

            if (existingRating.UserId != userId)
            {
                throw new InvalidOperationException("Niste autorizovani da obrišete ovu ocenu.");
            }

            _ratingRepository.Delete(id);
        }
    }
}