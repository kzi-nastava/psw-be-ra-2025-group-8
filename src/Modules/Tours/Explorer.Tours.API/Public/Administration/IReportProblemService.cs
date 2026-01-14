using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Administration
{
    public interface IReportProblemService
    {
        PagedResult<ReportProblemDto> GetPaged(int page, int pageSize);
        ReportProblemDto Create(ReportProblemDto entity);
        ReportProblemDto Update(ReportProblemDto entity);
        void Delete(long id);

        // Author response and tourist resolution
        ReportProblemDto AuthorRespond(int reportId, int authorId, string response);
        ReportProblemDto MarkResolved(int reportId, bool resolved, string? comment);

        // Message operations
        IssueMessageDto AddMessage(int reportId, int authorId, string content);
        List<IssueMessageDto> GetMessages(int reportId);
        ReportProblemDto GetById(int reportId);

        //Deadline operations
        ReportProblemDto SetDeadline(int reportId, DateTime deadline);
        ReportProblemDto CloseIssueByAdmin(int reportId);
        ReportProblemDto PenalizeAuthor(int reportId);
    }
}
