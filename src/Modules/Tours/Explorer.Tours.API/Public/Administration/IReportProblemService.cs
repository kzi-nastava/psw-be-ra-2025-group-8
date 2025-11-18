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
    }
}
