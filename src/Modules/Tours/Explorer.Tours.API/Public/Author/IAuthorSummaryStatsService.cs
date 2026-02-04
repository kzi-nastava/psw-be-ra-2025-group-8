using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Author;

public interface IAuthorSummaryStatsService
{
    AuthorQuickStatsDto GetQuickStats(int authorId);
    AuthorTourTrendsDto GetRatingTrends(int authorId);
}
