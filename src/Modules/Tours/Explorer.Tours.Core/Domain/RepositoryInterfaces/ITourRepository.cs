using System.Collections.Generic;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourRepository
{
    Tour Get(long id);
    Tour Create(Tour tour);
    Tour Update(Tour tour);
    void Delete(long id);

    List<Tour> GetByAuthor(int authorId);
    List<Tour> GetAll();
    //Maksim: Dodao sam Get po ID-ju zato sto su mi potrebni podaci Tour-a za ShoppingCart
    Tour GetById(long id);

    List<Tour> GetByIds(IEnumerable<long> ids);

    List<Tour> SearchByLocation(double latitude, double longitude, double distanceInKm);
}