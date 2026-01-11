using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Tests;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Tours.Infrastructure.Database;
using Explorer.Tours.Tests.TestHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Explorer.Stakeholders.API.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;


namespace Explorer.Tours.Tests;

public class ToursTestFactory : BaseTestFactory<ToursContext>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var serviceProvider = ReplaceNeededDbContexts(services).BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var logger = scopedServices.GetRequiredService<ILogger<ToursTestFactory>>();

            // Initialize Stakeholders database (for Person entities)
            var stakeholdersDb = scopedServices.GetRequiredService<StakeholdersContext>();
            var stakeholdersPath = Path.Combine(".", "..", "..", "..", "..", "Stakeholders", "Explorer.Stakeholders.Tests", "TestData");
            InitializeDatabase(stakeholdersDb, stakeholdersPath, logger);

            // Initialize Tours database (primary context)
            var toursDb = scopedServices.GetRequiredService<ToursContext>();
            var toursPath = Path.Combine(".", "..", "..", "..", "TestData");
            InitializeDatabase(toursDb, toursPath, logger);
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

        var personServiceDescriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(IInternalPersonService)
        );
        if (personServiceDescriptor != null)
            services.Remove(personServiceDescriptor);

        services.AddScoped<IInternalPersonService, MockInternalPersonService>();


        return services;
    }

}
