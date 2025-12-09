using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Tests;
using Explorer.Tours.Infrastructure.Database;
using Explorer.Tours.Tests.TestHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Explorer.Tours.Tests;

public class ToursTestFactory : BaseTestFactory<ToursContext>
{
    protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ToursContext>));
        services.Remove(descriptor!);
        services.AddDbContext<ToursContext>(SetupTestContext());

        // Registruj mock notification service za testove
        services.AddScoped<IIssueNotificationService, MockIssueNotificationService>();

        return services;
    }
}
