using ConwayGameOfLife.Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConwayGameOfLife.Data;

public interface IConwayDbContext
{
    public DbSet<Board> Boards { get; set; }
    public DbSet<BoardExecution> BoardExecutions { get; set; }
}
