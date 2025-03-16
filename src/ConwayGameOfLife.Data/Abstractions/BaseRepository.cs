namespace ConwayGameOfLife.Data.Abstractions;

public abstract class BaseRepository
{
    private readonly IConwayDbContext _context;

    protected IConwayDbContext ConwayDbContext => _context;

    public BaseRepository(IConwayDbContext context) =>
        _context = context;
}
