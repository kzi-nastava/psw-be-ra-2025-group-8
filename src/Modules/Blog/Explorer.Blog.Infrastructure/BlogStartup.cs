using System;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.UseCases;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.Blog.Infrastructure.Database.Repositories;
using Explorer.Blog.Infrastructure.Database;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddScoped<IBlogPostService, BlogPostService>();
        }

        private static void SetupInfrastructure(IServiceCollection services)
        {
            services.AddScoped<IBlogPostRepository, BlogPostRepository>();

            services.AddDbContext<BlogContext>(opt =>
                opt.UseNpgsql(DbConnectionStringBuilder.Build("blog"),
                    x => x.MigrationsHistoryTable("__EFMigrationsHistory", "blog")));
        }
    }
}
