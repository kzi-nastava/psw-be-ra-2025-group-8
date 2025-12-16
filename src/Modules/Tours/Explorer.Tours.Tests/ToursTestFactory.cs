using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Tests;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Tours.Infrastructure.Database;
using Explorer.Tours.Tests.TestHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Explorer.Tours.Tests;

public class ToursTestFactory : BaseTestFactory<ToursContext>
{
    protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
    {
        //TOURS CONTEXT
        var toursDescriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<ToursContext>)
        );
        services.Remove(toursDescriptor!);
        services.AddDbContext<ToursContext>(SetupTestContext());

        //STAKEHOLDERS CONTEXT
        var stakeholdersDescriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<StakeholdersContext>)
        );
        if (stakeholdersDescriptor != null)
            services.Remove(stakeholdersDescriptor);

        services.AddDbContext<StakeholdersContext>(SetupTestContext());

        // Mock notification service
        services.AddScoped<IIssueNotificationService, MockIssueNotificationService>();

        return services;
    }

}
