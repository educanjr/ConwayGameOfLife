using ConwayGameOfLife.Data;
using ConwayGameOfLife.Web.Middleware;
using Microsoft.EntityFrameworkCore;

namespace ConwayGameOfLife.App.Configuration;

public static class StartUpActions
{
    public static void ApplyPendingMigrations(this IApplicationBuilder app)
    {
        var appService = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
        using var scope = appService.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            var dbContext = services.GetRequiredService<ConwayDbContext>() ??
                throw new InvalidCastException($"{nameof(ConwayDbContext)} is not setup");

            dbContext.Database.Migrate();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating the database.");

            throw;
        }
    }

    public static void RegisterMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExeptionProcessMiddleware>();
    }
}
