using Explorer.API.Middleware;
using Explorer.API.Startup;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.ConfigureSwagger(builder.Configuration);

const string corsPolicy = "_corsPolicy";
builder.Services.ConfigureCors(corsPolicy);
builder.Services.ConfigureAuth();

builder.Services.RegisterModules();

var app = builder.Build();

// Global exception handler
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
app.UseSwagger();
app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseRouting();
app.UseCors(corsPolicy);
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Required for automated tests
namespace Explorer.API
{
    public partial class Program { }
}
