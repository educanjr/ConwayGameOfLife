using ConwayGameOfLife.App.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Read URLs from launchSettings.json
var urls = builder.Configuration["applicationUrl"];

// Detect if running inside a Container
var isContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

// If running in Docker, force HTTP-only (no HTTPS inside Docker)
if (isContainer)
{
    urls = "http://+:80";
}

// Apply URLs to the application
if (!string.IsNullOrEmpty(urls))
{
    builder.WebHost.UseUrls(urls.Split(";"));
}

// Add services to the container.
builder.Services.InstallServices(builder.Configuration, typeof(IServiceInstaller).Assembly);

var app = builder.Build();

app.ApplyPendingMigrations();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!isContainer)
{
    app.UseHttpsRedirection();
}

app.MapControllers();

app.Run();

public partial class Program
{
}
