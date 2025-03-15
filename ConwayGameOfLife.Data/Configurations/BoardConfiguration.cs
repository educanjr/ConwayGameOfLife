using ConwayGameOfLife.Application.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

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
                p => JsonConvert.SerializeObject(p),
                p => JsonConvert.DeserializeObject<BoardState>(p) ?? default!);

        //TODO: Set board initial seeding data with HasData
    }
}
