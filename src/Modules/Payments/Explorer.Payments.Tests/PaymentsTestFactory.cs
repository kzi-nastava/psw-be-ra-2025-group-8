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
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Explorer.Payments.Tests;

public class PaymentsTestFactory : BaseTestFactory<PaymentsContext>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var serviceProvider = ReplaceNeededDbContexts(services).BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var logger = scopedServices.GetRequiredService<ILogger<PaymentsTestFactory>>();

            // Initialize Stakeholders database (for Wallets dependency)
            var stakeholdersDb = scopedServices.GetRequiredService<StakeholdersContext>();
            var stakeholdersPath = Path.Combine(".", "..", "..", "..", "..", "Stakeholders", "Explorer.Stakeholders.Tests", "TestData");
            InitializeDatabase(stakeholdersDb, stakeholdersPath, logger);

            // Initialize Tours database (for Tour price lookups)
            var toursDb = scopedServices.GetRequiredService<ToursContext>();
            var toursPath = Path.Combine(".", "..", "..", "..", "..", "Tours", "Explorer.Tours.Tests", "TestData");
            InitializeDatabase(toursDb, toursPath, logger);

            // Initialize Payments database (primary context)
            var paymentsDb = scopedServices.GetRequiredService<PaymentsContext>();
            var paymentsPath = Path.Combine(".", "..", "..", "..", "TestData");
            InitializeDatabase(paymentsDb, paymentsPath, logger);
        });
    }

    private static void InitializeDatabase(DbContext context, string scriptFolder, ILogger logger)
    {
        try
        {
            context.Database.EnsureCreated();
            var databaseCreator = context.Database.GetService<IRelationalDatabaseCreator>();
            databaseCreator.CreateTables();
        }
        catch (Exception)
        {
            // CreateTables throws an exception if the schema already exists. This is a workaround for multiple dbcontexts.
        }

        try
        {
            if (!Directory.Exists(scriptFolder))
            {
                logger.LogWarning("Test data folder not found: {Folder}", scriptFolder);
                return;
            }

            var scriptFiles = Directory.GetFiles(scriptFolder, "*.sql");
            Array.Sort(scriptFiles);
            var script = string.Join('\n', scriptFiles.Select(File.ReadAllText));
            context.Database.ExecuteSqlRaw(script);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred seeding the database with test data from {Folder}. Error: {Message}", scriptFolder, ex.Message);
        }
    }

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
