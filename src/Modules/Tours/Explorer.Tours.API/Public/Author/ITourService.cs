using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Author
{
    public interface ITourService
    {
        PagedResult<TourDto> GetPaged(int page, int pageSize);
        TourDto Create(TourDto tour);
        TourDto Update(TourDto tour);
        List<TourDto> GetByAuthor(int authorId);
        void Delete(long id, int authorId);

        TourDto AddKeyPoint(long tourId, KeyPointDto keyPoint, int authorId);
        TourDto Publish(long tourId, int authorId);
        TourDto Archive(long tourId, int authorId);
        TourDto AddEquipment(long tourId, long equipmentId, int authorId);
        TourDto RemoveEquipment(long tourId, long equipmentId, int authorId);
        TourDto AddTag(long tourId, string tag, int authorId);
        TourDto RemoveTag(long tourId, string tag, int authorId);
        TourDto UpdateTags(long tourId, List<string> tags, int authorId);
        //Maksim: Dodao sam Get po ID-ju zato sto su mi potrebni podaci Tour-a za ShoppingCart
        TourDto GetById(long id);
    }
}