using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Internal;

public interface IInternalTourService
{
    string GetTourNameById(int tourId);
    string GetKeyPointNameByTourAndOrder(int tourId, int order);
    KeyPointDto GetKeyPointByTourAndOrder(int tourId, int order);
}
