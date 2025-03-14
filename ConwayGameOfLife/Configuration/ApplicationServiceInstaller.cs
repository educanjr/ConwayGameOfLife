namespace ConwayGameOfLife.App.Configuration;

public class ApplicationServiceInstallerpublic : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(mediatrService =>
        {
            mediatrService.RegisterServicesFromAssembly(Application.AssemblyReference.Assembly);
        });
    }
}
