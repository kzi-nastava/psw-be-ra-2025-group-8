using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.API.Public.PersonalEquipment;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.API.Public.Author;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Mappers;
using Explorer.Tours.Core.UseCases.Administration;
using Explorer.Tours.Core.UseCases.PersonalEquipment;
using Explorer.Tours.Core.UseCases.Tourist;
using Explorer.Tours.Infrastructure.Database;
using Explorer.Tours.Infrastructure.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace Explorer.Tours.Infrastructure;

public static class ToursStartup
{
    public static IServiceCollection ConfigureToursModule(this IServiceCollection services)
    {
        // Registers all profiles since it works on the assembly
        services.AddAutoMapper(typeof(ToursProfile).Assembly);
        SetupCore(services);
        SetupInfrastructure(services);
        return services;
    }

    private static void SetupCore(IServiceCollection services)
    {
        services.AddScoped<IEquipmentService, EquipmentService>();
        services.AddScoped<IMonumentService, MonumentService>();
        services.AddScoped<IFacilityService, FacilityService>();
        services.AddScoped<IReportProblemService, ReportProblemService>();
        services.AddScoped<IPersonEquipmentService, PersonEquipmentService>();
        services.AddScoped<ITourService, TourService>();
        services.AddScoped<IPositionService, PositionService>();
        services.AddScoped<IObjectService, ObjectService>();
        //Minja dodao ovo
        services.AddScoped<ITouristPreferencesService, TouristPreferencesService>();
        services.AddScoped<ITransportTypePreferencesService, TransportTypePreferencesService>();
        services.AddScoped<IPreferenceTagsService, PreferenceTagsService>();
        //

        services.AddScoped<ITourExecutionService, TourExecutionService>();
        services.AddScoped<ITourRatingService, TourRatingService>();
        services.AddScoped<ITouristTourService, TouristTourService>();
    }

    private static void SetupInfrastructure(IServiceCollection services)
    {
        services.AddScoped<IPersonEquipmentRepository, PersonEquipmentRepository>();
        services.AddScoped<IMonumentRepository, MonumentDbRepository>();
        services.AddScoped<IPositionRepository, PositionRepository>();
        services.AddScoped<IReportProblemRepository, ReportProblemRepository>();
        //Minja dodao ovo
        services.AddScoped<ITouristPreferencesRepository, TouristPreferencesRepository>();
        services.AddScoped<ITransportTypePreferencesRepository, TransportTypePreferencesRepository>();
        services.AddScoped<ITagsRepository, TagsRepository>();
        services.AddScoped<IPreferenceTagsRepository, PreferenceTagsRepository>();

        services.AddScoped<ITourExecutionRepository, TourExecutionRepository>();
        services.AddScoped<IKeyPointReachedRepository, KeyPointReachedRepository>();
        services.AddScoped<IKeyPointRepository, KeyPointRepository>();
        services.AddScoped(typeof(ICrudRepository<Equipment>), typeof(CrudDatabaseRepository<Equipment, ToursContext>));
        services.AddScoped(typeof(ICrudRepository<Facility>), typeof(CrudDatabaseRepository<Facility, ToursContext>));
        services.AddScoped(typeof(ICrudRepository<Tour>), typeof(CrudDatabaseRepository<Tour, ToursContext>));
        services.AddScoped(typeof(ICrudRepository<TourExecution>), typeof(CrudDatabaseRepository<TourExecution, ToursContext>));
        services.AddScoped<ITourRepository, TourRepository>();
        services.AddScoped(typeof(ICrudRepository<ReportProblem>), typeof(CrudDatabaseRepository<ReportProblem, ToursContext>));
        services.AddScoped<ITourRatingRepository, TourRatingRepository>();

        services.AddDbContext<ToursContext>(opt =>
            opt.UseNpgsql(DbConnectionStringBuilder.Build("tours"),
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "tours")));
    }
}