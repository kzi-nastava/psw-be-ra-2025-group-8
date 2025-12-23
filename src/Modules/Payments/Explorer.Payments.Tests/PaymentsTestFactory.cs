using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Tests;
using Explorer.Payments.Core.UseCases;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Payments.Tests.TestHelpers;
using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Explorer.Tours.API.Public.Author;

namespace Explorer.Payments.Tests;

public class PaymentsTestFactory : BaseTestFactory<PaymentsContext>
{
    protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
    {
        // PAYMENTS CONTEXT
        var paymentsDescriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<PaymentsContext>)
        );
        if (paymentsDescriptor != null)
            services.Remove(paymentsDescriptor);
        services.AddDbContext<PaymentsContext>(SetupTestContext());

        // STAKEHOLDERS CONTEXT
        var stakeholdersDescriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<StakeholdersContext>)
        );
        if (stakeholdersDescriptor != null)
            services.Remove(stakeholdersDescriptor);

        services.AddDbContext<StakeholdersContext>(SetupTestContext());

        // TOURS CONTEXT
        var toursDescriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<ToursContext>)
        );
        if (toursDescriptor != null)
            services.Remove(toursDescriptor);

        services.AddDbContext<ToursContext>(SetupTestContext());

        // register mock provider and mock tour service for tests
        services.AddScoped<ITourPriceProvider, MockTourPriceProvider>();
        services.AddScoped<ITourService, MockTourService>();

        return services;
    }
}
