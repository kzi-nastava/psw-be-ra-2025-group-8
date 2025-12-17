using System;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.UseCases;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.Blog.Infrastructure.Database.Repositories;
using Explorer.Blog.Infrastructure.Database;
using Explorer.Blog.Core.Mappers;
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

            services.AddAutoMapper(typeof(BlogProfile));

            SetupCore(services);
            SetupInfrastructure(services);
            return services;
        }

        private static void SetupCore(IServiceCollection services)
        {
            services.AddScoped<IBlogPostService, BlogPostService>();
            services.AddScoped<IBlogCommentService, BlogCommentService>();
        }

        private static void SetupInfrastructure(IServiceCollection services)
        {
        services.AddScoped<IBlogPostRepository, BlogPostRepository>();
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("blog"));
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

            services.AddDbContext<BlogContext>(opt =>
                opt.UseNpgsql(DbConnectionStringBuilder.Build("blog"),
                    x => x.MigrationsHistoryTable("__EFMigrationsHistory", "blog")));
        }
    }
}
