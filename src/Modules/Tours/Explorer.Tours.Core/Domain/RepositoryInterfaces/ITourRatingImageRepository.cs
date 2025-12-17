using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface ITourRatingImageRepository
    {
        TourRatingImage Create(TourRatingImage image);
        List<TourRatingImage> GetByTourRatingId(long tourRatingId);
        TourRatingImage Get(long id);
        void Delete(long id);
    }
}
