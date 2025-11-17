using System;
using Explorer.Blog.Infrastructure.Database;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Explorer.Blog.Infrastructure
{
    public static class BlogStartup
    {
        public static IServiceCollection ConfigureBlogModule(this IServiceCollection services)
        {
            // Registruje sve AutoMapper profile iz svih učitanih asembli-ja
            // više nam ne treba BlogProfile tip, pa nema CS0246 greške
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            SetupCore(services);
            SetupInfrastructure(services);
            return services;
        }

        private static void SetupCore(IServiceCollection services)
        {
            // ovde ide registracija Core servisa za Blog modul (trenutno prazno)
        }

        private static void SetupInfrastructure(IServiceCollection services)
        {
            var dataSourceBuilder =
                new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("blog"));
            dataSourceBuilder.EnableDynamicJson();
            var dataSource = dataSourceBuilder.Build();

            services.AddDbContext<BlogContext>(opt =>
                opt.UseNpgsql(
                    dataSource,
                    x => x.MigrationsHistoryTable("__EFMigrationsHistory", "blog")));
        }
    }
}
