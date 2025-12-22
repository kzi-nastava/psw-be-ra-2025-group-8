using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Tests;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Explorer.Payments.Tests;

public class PaymentsTestFactory : BaseTestFactory<PaymentsContext>
{
    protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
    {
        //PAYMENTS CONTEXT
        var paymentsDescriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<PaymentsContext>)
        );
        services.Remove(paymentsDescriptor!);
        services.AddDbContext<ToursContext>(SetupTestContext());

        //STAKEHOLDERS CONTEXT
        var stakeholdersDescriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<StakeholdersContext>)
        );
        if (stakeholdersDescriptor != null)
            services.Remove(stakeholdersDescriptor);

        services.AddDbContext<StakeholdersContext>(SetupTestContext());

        //TOURS CONTEXT
        var toursDescriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<ToursContext>)
        );
        if (toursDescriptor != null)
            services.Remove(toursDescriptor);

        services.AddDbContext<ToursContext>(SetupTestContext());

        

        return services;
    }
}
