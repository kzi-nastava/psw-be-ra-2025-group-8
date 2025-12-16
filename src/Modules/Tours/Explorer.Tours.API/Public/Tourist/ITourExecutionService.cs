using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist;

public interface ITourExecutionService
{
    PagedResult<TourExecutionDto> GetPaged(int page, int pageSize);
    TourExecutionDto Get(int id);
    TourExecutionDto Create(TourExecutionDto tourExecutionDto);
    TourExecutionDto Update(TourExecutionDto tourExecutionDto);
    TourExecutionDto GetByTouristAndTour(int touristId, int tourId);
    List<TourExecutionDto> GetByTourist(int touristId);
    List<TourExecutionDto> GetByTour(int tourId);
    void Delete(int id);
    CheckKeyPointResponseDto CheckKeyPoint(CheckKeyPointRequestDto request);
    List<KeyPointReachedDto> GetReachedKeyPoints(long tourExecutionId);
    KeyPointSecretDto GetKeyPointSecret(long tourExecutionId, int keyPointOrder);
}