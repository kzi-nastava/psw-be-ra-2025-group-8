using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class ReportProblemRepository : IReportProblemRepository
{
    private readonly ToursContext _context;

    public ReportProblemRepository(ToursContext context)
    {
        _context = context;
    }

    public ReportProblem GetWithMessages(long id)
    {
        var report = _context.ReportProblem
            .Include(r => r.Messages)
            .FirstOrDefault(r => r.Id == id);

        if (report == null) throw new NotFoundException("Report problem not found: " + id);

        return report;
    }
}
