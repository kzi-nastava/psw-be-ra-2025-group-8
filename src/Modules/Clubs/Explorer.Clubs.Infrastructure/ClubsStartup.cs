using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Clubs.API.Public;
using Explorer.Clubs.Core.Domain.RepositoryInterfaces;
using Explorer.Clubs.Core.Mappers;
using Explorer.Clubs.Core.UseCases;
using Explorer.Clubs.Infrastructure.Database;
using Explorer.Clubs.Infrastructure.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Explorer.Clubs.Infrastructure
{
    public static class ClubsStartup
    {
        public static IServiceCollection ConfigureClubsModule(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(ClubsProfile).Assembly);
            SetupCore(services);
            SetupInfrastructure(services);
            return services;
        }

        private static void SetupCore(IServiceCollection services)
        {
            services.AddScoped<IClubService, ClubService>();
        }

        private static void SetupInfrastructure(IServiceCollection services)
        {
            services.AddScoped<IClubRepository, ClubDbRepository>();

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("clubs"));
            dataSourceBuilder.EnableDynamicJson();
            var dataSource = dataSourceBuilder.Build();

            services.AddDbContext<ClubsContext>(opt =>
                opt.UseNpgsql(
                    dataSource,
                    x => x.MigrationsHistoryTable("__EFMigrationsHistory", "clubs")
                ));
        }
    }
}
