using ConwayGameOfLife.Application.Entities;
using ConwayGameOfLife.Data.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
                p => DataConversionHelper.SerializeBoardState(p),
                p => DataConversionHelper.DeserializeBoardState(p));

        //Step must not be repeated by Board, Step number is used to find Execution
        builder
            .HasIndex(b => new { b.BoardId, b.Step })
            .IsUnique();
    }
}
