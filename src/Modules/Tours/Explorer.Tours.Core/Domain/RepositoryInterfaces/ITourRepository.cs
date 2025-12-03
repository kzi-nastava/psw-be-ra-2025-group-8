namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourRepository
{
    List<Tour> GetByAuthor(int authorId);
    //Maksim: Dodao sam Get po ID-ju zato sto su mi potrebni podaci Tour-a za ShoppingCart
    Tour Get(long id);
}