using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ConwayGameOfLife.Data;

public class ConwayDbContextFactory : IDesignTimeDbContextFactory<ConwayDbContext>
{
    public ConwayDbContext CreateDbContext(string[] args)
    {
        //Ensure the correct base directory
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../ConwayGameOfLife.App");

        var builder = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);

        var config = builder
            .Build();
        var connectionString = config.GetConnectionString("ConwayDatabase");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Could not find the connection string. Check your appsettings.json.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<ConwayDbContext>();
        

        optionsBuilder.UseNpgsql(connectionString, sqlOpt =>
            sqlOpt.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorCodesToAdd: null));

        var dbSettings = new DbSettings(config.GetConnectionString("ConwayDatabase"));
        return new ConwayDbContext(optionsBuilder.Options, dbSettings);
    }
}
