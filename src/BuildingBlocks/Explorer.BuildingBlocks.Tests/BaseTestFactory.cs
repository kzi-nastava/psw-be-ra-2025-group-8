using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Explorer.API;
using System.IO;
using System;

namespace Explorer.BuildingBlocks.Tests;

public abstract class BaseTestFactory<TDbContext> : WebApplicationFactory<Program> where TDbContext : DbContext
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            using var scope = BuildServiceProvider(services).CreateScope();
            var scopedServices = scope.ServiceProvider;  
            var db = scopedServices.GetRequiredService<TDbContext>();
            var logger = scopedServices.GetRequiredService<ILogger<BaseTestFactory<TDbContext>>>();

            var path = Path.Combine(".", "..", "..", "..", "TestData");
            InitializeDatabase(db, path, logger);
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
            var scriptFiles = Directory.GetFiles(scriptFolder);
            // Sort by filename only (not full path) to preserve intended ordering like a-..., b-..., c-... across modules
            Array.Sort(scriptFiles, (a, b) => string.Compare(Path.GetFileName(a), Path.GetFileName(b), StringComparison.OrdinalIgnoreCase));

            // Execute each script file; split into individual statements and execute separately
            foreach (var file in scriptFiles)
            {
                try
                {
                    var script = File.ReadAllText(file);
                    if (string.IsNullOrWhiteSpace(script)) continue;

                    // Split by semicolon to get individual statements. This is a simple splitter suitable for our seed scripts.
                    var statements = script.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var stmt in statements)
                    {
                        var sql = stmt.Trim();
                        if (string.IsNullOrWhiteSpace(sql)) continue;

                        try
                        {
                            context.Database.ExecuteSqlRaw(sql);
                        }
                        catch (Exception exStmt)
                        {
                            logger.LogError(exStmt, "Error executing SQL statement from file '{File}': {Message}\nStatement: {Stmt}", Path.GetFileName(file), exStmt.Message, sql.Length > 200 ? sql.Substring(0, 200) + "..." : sql);
                            // continue with next statement
                        }
                    }
                }
                catch (Exception exFile)
                {
                    logger.LogError(exFile, "Error executing SQL script file '{File}': {Message}", Path.GetFileName(file), exFile.Message);
                    // continue with next script
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred seeding the database with test data. Error: {Message}", ex.Message);
        }
    }

    private ServiceProvider BuildServiceProvider(IServiceCollection services)
    {
        return ReplaceNeededDbContexts(services).BuildServiceProvider();
    }

    protected abstract IServiceCollection ReplaceNeededDbContexts(IServiceCollection services);

    protected static Action<DbContextOptionsBuilder> SetupTestContext()
    {
        var server = Environment.GetEnvironmentVariable("DATABASE_HOST") ?? "localhost";
        var port = Environment.GetEnvironmentVariable("DATABASE_PORT") ?? "5432";
        var database = Environment.GetEnvironmentVariable("DATABASE_SCHEMA") ?? "explorer-v1-test";
        var user = Environment.GetEnvironmentVariable("DATABASE_USERNAME") ?? "postgres";
        var password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD") ?? "root";
        var pooling = Environment.GetEnvironmentVariable("DATABASE_POOLING") ?? "true";

        var connectionString = $"Server={server};Port={port};Database={database};User ID={user};Password={password};Pooling={pooling};Include Error Detail=True";

        return opt => opt.UseNpgsql(connectionString);
    }
}