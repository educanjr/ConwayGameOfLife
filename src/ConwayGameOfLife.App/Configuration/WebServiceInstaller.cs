using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ConwayGameOfLife.App.Configuration;

public class WebServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddApiVersioning(opt =>
            {
                opt.ReportApiVersions = true;
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1);
                opt.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(opt =>
            {
                opt.GroupNameFormat = "'v'VVV"; //Formats versions as v1, v2, etc.
                opt.SubstituteApiVersionInUrl = true;
            });

        services.AddEndpointsApiExplorer();

        services
            .AddControllers()
            .AddApplicationPart(Web.AssemblyReference.Assembly);

        services.AddSwaggerGen(opt =>
        {
            var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

            foreach (var description in provider.ApiVersionDescriptions)
            {
                opt.SwaggerDoc(description.GroupName, new OpenApiInfo
                {
                    Title = $"Conway's Game Of Life API {description.ApiVersion}",
                    Version = description.ApiVersion.ToString()
                });
            }
        });

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",
                builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });

        services.AddValidatorsFromAssembly(Web.AssemblyReference.Assembly);
        services.AddFluentValidationAutoValidation();
    }
}

//Custom classes for versioning
public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;
    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => _provider = provider;

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = $"Conway Game Of Life API {description.ApiVersion}",
                Version = description.ApiVersion.ToString()
            });
        }
    }
}
