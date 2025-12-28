using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.API.Internal;
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
using Npgsql;


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
        services.AddScoped<IInternalPersonService, PersonService>();
        services.AddScoped<IInternalUserService, InternalUserService>();
        services.AddScoped<ITokenGenerator, JwtGenerator>();


        services.AddScoped<IRatingService, RatingService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IMeetupService, MeetupService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<BuildingBlocks.Core.UseCases.IIssueNotificationService, IssueNotificationServiceAdapter>();
        services.AddScoped<IFollowerService, FollowerService>();

        //za klubove
        services.AddScoped<IClubService, ClubService>();
        services.AddScoped<IClubMessageService, ClubMessageService>();

        services.AddScoped<IWalletService, WalletService>();
        services.AddScoped<IInternalWalletService, WalletService>();
    }

    private static void SetupInfrastructure(IServiceCollection services)
    {
        services.AddScoped(typeof(ICrudRepository<Person>), typeof(CrudDatabaseRepository<Person, StakeholdersContext>));
        services.AddScoped<IUserRepository, UserDatabaseRepository>();
        services.AddScoped<IRatingRepository, RatingDbRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IMeetupRepository, MeetupRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IFollowerRepository, FollowerRepository>();
        services.AddScoped<IFollowerMessageRepository, FollowerMessageRepository>();


        //za klubove
        services.AddScoped<IClubRepository, ClubDbRepository>();
        services.AddScoped<IClubJoinRequestRepository, ClubJoinRequestRepository>();
        services.AddScoped<IClubInvitationRepository, ClubInvitationRepository>();
        services.AddScoped<IClubMessageRepository, ClubMessageRepository>();

        services.AddScoped<IWalletRepository, WalletDatabaseRepository>();

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("stakeholders"));
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<StakeholdersContext>(opt =>
            opt.UseNpgsql(DbConnectionStringBuilder.Build("stakeholders"),
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "stakeholders")));
    }
}
