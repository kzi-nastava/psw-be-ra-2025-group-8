using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Public.Tourist
{
    public interface ITourRatingService
    {
        PagedResult<TourRatingDto> GetPaged(int page, int pageSize);
        TourRatingDto Get(long id);
        TourRatingDto Create(TourRatingDto tourRatingDto);
        TourRatingDto Update(TourRatingDto tourRatingDto);
        TourRatingDto GetByTouristAndTour(int touristId, int tourId);
        List<TourRatingDto> GetByTourist(int touristId);
        List<TourRatingDto> GetByTour(int tourId);
        void Delete(long id);
    }
}
