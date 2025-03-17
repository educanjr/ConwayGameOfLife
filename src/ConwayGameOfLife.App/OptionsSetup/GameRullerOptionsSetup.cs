using ConwayGameOfLife.Application.ConfigOptions;
using Microsoft.Extensions.Options;

namespace ConwayGameOfLife.App.OptionsSetup;

public class GameRullerOptionsSetup : IConfigureOptions<GameRullerConfig>
{
    private const string SectionName = "GameRuller";
    private readonly IConfiguration _configuration;

    public GameRullerOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(GameRullerConfig options)
    {
        _configuration.GetSection(SectionName).Bind(options);
    }
}
