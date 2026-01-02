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
using Npgsql;

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

        // Mirror production (PaymentsStartup) JSON mapping setup
        var paymentsDataSourceBuilder = new NpgsqlDataSourceBuilder(BuildTestConnectionString());
        paymentsDataSourceBuilder.EnableDynamicJson();
        var paymentsDataSource = paymentsDataSourceBuilder.Build();
        services.AddDbContext<PaymentsContext>(opt => opt.UseNpgsql(paymentsDataSource));

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

        // existing mocks
        services.AddScoped<ITourPriceProvider, MockTourPriceProvider>();
        services.AddScoped<ITourService, MockTourService>();

        // mock bundle provider
        var bundleInfoProviderDescriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(IBundleInfoProvider)
        );
        if (bundleInfoProviderDescriptor != null)
            services.Remove(bundleInfoProviderDescriptor);

        services.AddScoped<IBundleInfoProvider, MockBundleInfoProvider>();

        return services;
    }

    private static string BuildTestConnectionString()
    {
        var server = Environment.GetEnvironmentVariable("DATABASE_HOST") ?? "localhost";
        var port = Environment.GetEnvironmentVariable("DATABASE_PORT") ?? "5432";
        var database = Environment.GetEnvironmentVariable("DATABASE_SCHEMA") ?? "explorer-v1-test";
        var user = Environment.GetEnvironmentVariable("DATABASE_USERNAME") ?? "postgres";
        var password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD") ?? "root";
        var pooling = Environment.GetEnvironmentVariable("DATABASE_POOLING") ?? "true";

        return $"Server={server};Port={port};Database={database};User ID={user};Password={password};Pooling={pooling};Include Error Detail=True";
    }
}
