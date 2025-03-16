using ConwayGameOfLife.Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConwayGameOfLife.Data;

public interface IConwayDbContext : IDisposable
{
    DbSet<Board> Boards { get; set; }
    DbSet<BoardExecution> BoardExecutions { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
