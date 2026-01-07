using Explorer.API.Middleware;
using Explorer.API.Startup;
using Explorer.API.Hubs;
using Explorer.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.ConfigureSwagger(builder.Configuration);

const string corsPolicy = "_corsPolicy";
builder.Services.ConfigureCors(corsPolicy);
builder.Services.ConfigureAuth();

builder.Services.RegisterModules();

// SignalR configuration
builder.Services.AddSignalR();
builder.Services.AddScoped<IChatNotificationService, ChatNotificationService>();

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

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(corsPolicy);


app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

app.Run();

// Required for automated tests
namespace Explorer.API
{
    public partial class Program { }
}
