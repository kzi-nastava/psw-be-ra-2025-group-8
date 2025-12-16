using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Explorer.Tours.Core.Domain.TourExecution;

namespace Explorer.Tours.Core.UseCases.Tourist
{
    public class TourRatingService : ITourRatingService
    {
        private readonly ITourRatingRepository _tourRatingRepository;
        private readonly IMapper _mapper;

        public TourRatingService(
            ITourRatingRepository tourRatingRepository,
            IMapper mapper)
        {
            _tourRatingRepository = tourRatingRepository;
            _mapper = mapper;
        }

        public PagedResult<TourRatingDto> GetPaged(int page, int pageSize)
        {
            var result = _tourRatingRepository.GetPaged(page, pageSize);
            var items = result.Results.Select(_mapper.Map<TourRatingDto>).ToList();
            return new PagedResult<TourRatingDto>(items, result.TotalCount);
        }

        public TourRatingDto Get(long id)
        {
            var tourRating = _tourRatingRepository.Get(id);
            return tourRating != null ? _mapper.Map<TourRatingDto>(tourRating) : null;
        }

        public TourRatingDto Create(TourRatingDto tourRatingDto)
        {
            var tourRating = new TourRating(
                tourRatingDto.IdTour,
                tourRatingDto.IdTourist,
                tourRatingDto.Rating,
                tourRatingDto.Comment,
                tourRatingDto.TourCompletionPercentage
            );

            var result = _tourRatingRepository.Create(tourRating);
            return _mapper.Map<TourRatingDto>(result);
        }

        public TourRatingDto Update(TourRatingDto tourRatingDto)
        {
            var existing = _tourRatingRepository.Get(tourRatingDto.Id);
            if (existing == null)
                throw new KeyNotFoundException($"TourExecution with id {tourRatingDto.Id} not found.");

            existing.UpdateRating(tourRatingDto.Rating);
            existing.UpdateComment(tourRatingDto.Comment);
            existing.UpdateTourCompletionPercentage(tourRatingDto.TourCompletionPercentage);

            var result = _tourRatingRepository.Update(existing);
            return _mapper.Map<TourRatingDto>(result);
        }

        public TourRatingDto GetByTouristAndTour(int touristId, int tourId)
        {
            var tourRating = _tourRatingRepository.GetByTouristAndTour(touristId, tourId);
            return tourRating != null ? _mapper.Map<TourRatingDto>(tourRating) : null;
        }

        public List<TourRatingDto> GetByTourist(int touristId)
        {
            var ratings = _tourRatingRepository.GetByTourist(touristId);
            return ratings.Select(_mapper.Map<TourRatingDto>).ToList();
        }

        public List<TourRatingDto> GetByTour(int tourId)
        {
            var ratings = _tourRatingRepository.GetByTour(tourId);
            return ratings.Select(_mapper.Map<TourRatingDto>).ToList();
        }

        public void Delete(long id)
        {
            _tourRatingRepository.Delete(id);
        }
    }
}
