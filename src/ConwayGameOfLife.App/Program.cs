using ConwayGameOfLife.App.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Detect if running in a container
var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

// Force HTTP only when running in Container
var urls = isDocker ? "http://+:80" : "https://+:443;http://+:80";
builder.WebHost.UseUrls(urls);

// Add services to the container.
builder.Services.InstallServices(builder.Configuration, typeof(IServiceInstaller).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!isDocker)
{
    app.UseHttpsRedirection();
}

app.MapControllers();

app.Run();
