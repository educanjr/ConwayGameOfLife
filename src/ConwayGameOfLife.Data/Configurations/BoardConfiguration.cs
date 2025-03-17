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

        var blikerId = Guid.Parse("3fedc2a6-9743-4b8c-8087-c34cd0e383ad");
        var gliderGunId = Guid.Parse("2229a2ca-3e77-4637-91e1-06e66630068b");
        builder.HasData(
            new Board
            {
                Id = blikerId,
                Name = "Blinker 3x3",
                InitialState = BoardState.FromJaggedArray(new bool[][]
                {
                    new bool[] { false, false, false },
                    new bool[] { true, true, true },
                    new bool[] { false, false, false }
                })
            },
            new Board
            {
                Id = gliderGunId,
                Name = "Glider Gun 10x10",
                InitialState = BoardState.FromJaggedArray(new bool[][]
                {
                    new bool[] { false, false, false, false, false, false, false, false, false, false },
                    new bool[] { false, false, false, false, false, false, false, true, false, false },
                    new bool[] { false, false, false, false, false, false, false, false, true, false },
                    new bool[] { false, false, true, true, false, false, false, false, true, true },
                    new bool[] { false, false, true, true, false, false, false, false, false, false },
                    new bool[] { false, false, false, false, false, false, false, false, false, false },
                    new bool[] { false, false, false, false, false, false, false, false, false, false },
                    new bool[] { false, false, false, false, false, false, false, false, false, false },
                    new bool[] { false, false, false, false, false, false, false, false, false, false },
                    new bool[] { false, false, false, false, false, false, false, false, false, false }
                })
            });
    }
}
