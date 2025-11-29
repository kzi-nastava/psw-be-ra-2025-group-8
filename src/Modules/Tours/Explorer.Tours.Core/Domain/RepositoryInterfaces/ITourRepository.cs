using System.Collections.Generic;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourRepository
{
    Tour Get(long id);
    Tour Create(Tour tour);
    Tour Update(Tour tour);
    void Delete(long id);

    List<Tour> GetByAuthor(int authorId);
}
