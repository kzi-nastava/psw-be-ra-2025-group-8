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

        public RatingDto Create(RatingDto ratingDto)
        {
            var rating = _mapper.Map<Rating>(ratingDto);
            var result = _ratingRepository.Create(rating);

            return _mapper.Map<RatingDto>(result);
        }

        public PagedResult<RatingDto> GetPaged(int page, int pageSize)
        {
            var result = _ratingRepository.GetPaged(page, pageSize);

            var items = result.Results.Select(_mapper.Map<RatingDto>).ToList();

            return new PagedResult<RatingDto>(items, result.TotalCount);
        }

        public RatingDto Get(int id)
        {
            var result = _ratingRepository.Get(id);
            return _mapper.Map<RatingDto>(result);
        }

        public RatingDto Update(RatingDto ratingDto)
        {
            var rating = _mapper.Map<Rating>(ratingDto);
            var result = _ratingRepository.Update(rating);
            return _mapper.Map<RatingDto>(result);
        }

        public void Delete(int id)
        {
            _ratingRepository.Delete(id);
        }
    }
}