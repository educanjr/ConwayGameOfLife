using ConwayGameOfLife.Application.Entities;
using ConwayGameOfLife.Data.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConwayGameOfLife.Data.Configurations;

public class BoardConfiguration : IEntityTypeConfiguration<Board>
{
    public void Configure(EntityTypeBuilder<Board> builder)
    {
        builder
            .HasKey(x => x.Id);

        builder
            .HasMany(x => x.Executions)
            .WithOne()
            .HasForeignKey(x => x.BoardId);

        builder
            .Property(x => x.InitialState)
            .HasConversion(
                p => DataConversionHelper.SerializeBoardState(p),
                p => DataConversionHelper.DeserializeBoardState(p));

        //TODO: Set board initial seeding data with HasData
    }
}
