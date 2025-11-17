namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourRepository
{
    List<Tour> GetByAuthor(int authorId);
}