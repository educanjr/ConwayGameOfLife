using ConwayGameOfLife.Application.Entities;
using ConwayGameOfLife.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace ConwayGameOfLife.Data;

public class ConwayDbContext : DbContext, IConwayDbContext
{
    private readonly IDbSettings _dbSettings;
    public ConwayDbContext(IDbSettings dbSettings) : base() =>
        _dbSettings = dbSettings;

    public ConwayDbContext(DbContextOptions<ConwayDbContext> options, IDbSettings dbSettings) : base(options) =>
        _dbSettings = dbSettings;

    public DbSet<Board> Boards { get; set; }
    public DbSet<BoardExecution> BoardExecutions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BoardConfiguration());
        modelBuilder.ApplyConfiguration(new BoardExecutionConfiguration());
    }
}
