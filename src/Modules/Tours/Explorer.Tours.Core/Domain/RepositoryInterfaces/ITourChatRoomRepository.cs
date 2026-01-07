using System.Collections.Generic;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface ITourChatRoomRepository
    {
        TourChatRoom Create(TourChatRoom chatRoom);
        TourChatRoom? Get(long id);
        TourChatRoom? GetByTourId(long tourId);
        List<TourChatRoom> GetByUserId(long userId);
        TourChatRoom Update(TourChatRoom chatRoom);
        void Delete(long id);
        bool ExistsForTour(long tourId);
    }
}
