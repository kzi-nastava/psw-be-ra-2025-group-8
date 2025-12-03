namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface IReportProblemRepository
{
    ReportProblem GetWithMessages(long id);
}
