using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.Mappers;
using Explorer.Stakeholders.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Stakeholders.Infrastructure.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace Explorer.Stakeholders.Infrastructure;

public static class StakeholdersStartup
{
    public static IServiceCollection ConfigureStakeholdersModule(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(StakeholderProfile).Assembly);
        SetupCore(services);
        SetupInfrastructure(services);
        return services;
    }

    private static void SetupCore(IServiceCollection services)
    {
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IPersonService, PersonService>();
        services.AddScoped<ITokenGenerator, JwtGenerator>();

        //Minja dodao ovo
        services.AddScoped<ITouristPreferencesService, TouristPreferencesService>();
        services.AddScoped<ITransportTypePreferencesService, TransportTypePreferencesService>();
        services.AddScoped<IPreferenceTagsService, PreferenceTagsService>();
        //

        services.AddScoped<IRatingService, RatingService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IMeetupService, MeetupService>();
    }

    private static void SetupInfrastructure(IServiceCollection services)
    {
        services.AddScoped(typeof(ICrudRepository<Person>), typeof(CrudDatabaseRepository<Person, StakeholdersContext>));
        services.AddScoped<IUserRepository, UserDatabaseRepository>();
        services.AddScoped<IRatingRepository, RatingDbRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IMeetupRepository, MeetupRepository>();

        //Minja dodao ovo
        services.AddScoped<ITouristPreferencesRepository, TouristPreferencesRepository>();
        services.AddScoped<ITransportTypePreferencesRepository, TransportTypePreferencesRepository>();
        services.AddScoped<ITagsRepository, TagsRepository>();
        services.AddScoped<IPreferenceTagsRepository, PreferenceTagsRepository>();
        //

        services.AddDbContext<StakeholdersContext>(opt =>
            opt.UseNpgsql(DbConnectionStringBuilder.Build("stakeholders"),
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "stakeholders")));
    }
}
