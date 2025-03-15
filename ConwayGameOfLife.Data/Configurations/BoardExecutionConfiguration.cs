using ConwayGameOfLife.Application.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace ConwayGameOfLife.Data.Configurations;

class BoardExecutionConfiguration : IEntityTypeConfiguration<BoardExecution>
{
    public void Configure(EntityTypeBuilder<BoardExecution> builder)
    {
        builder
            .HasKey(x => x.Id);

        builder
            .Property(x => x.State)
            .HasConversion(
                p => JsonConvert.SerializeObject(p),
                p => JsonConvert.DeserializeObject<BoardState>(p) ?? default!);

        //Step must not be repeated by Board, Step number is used to find Execution
        builder
            .HasIndex(b => new { b.BoardId, b.Step })
            .IsUnique();
    }
}
