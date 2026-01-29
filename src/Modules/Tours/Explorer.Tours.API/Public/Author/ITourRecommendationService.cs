using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Author;

public interface ITourRecommendationService
{
    AuthorRecommendationsDto GetRecommendationsForAuthor(int authorId);
}
