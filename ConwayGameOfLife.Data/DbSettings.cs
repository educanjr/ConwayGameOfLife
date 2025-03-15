namespace ConwayGameOfLife.Data;

public sealed class DbSettings : IDbSettings
{
    private readonly string? _connectionString;
    public string ConnectionString => _connectionString ?? throw new ApplicationException("Connection string was not provided");

    public DbSettings()
    {
        _connectionString = string.Empty;
    }

    public DbSettings(string? connectionString)
    {
        _connectionString = connectionString;
    }
}
