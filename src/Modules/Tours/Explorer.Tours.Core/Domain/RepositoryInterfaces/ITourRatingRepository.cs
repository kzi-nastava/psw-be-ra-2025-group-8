using Explorer.BuildingBlocks.Core.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface ITourRatingRepository
    {
        PagedResult<TourRating> GetPaged(int page, int pageSize);
        TourRating Create(TourRating tourRating);
        TourRating Update(TourRating tourRating);
        TourRating? Get(long id);
        TourRating? GetByTouristAndTour(int touristId, int tourId);
        List<TourRating> GetByTourist(int touristId);
        List<TourRating> GetByTour(int tourId);
        void Delete(long id);
    }
}
