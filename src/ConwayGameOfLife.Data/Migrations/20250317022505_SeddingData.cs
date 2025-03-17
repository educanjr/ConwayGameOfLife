using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ConwayGameOfLife.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeddingData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Boards",
                columns: new[] { "Id", "InitialState", "Name" },
                values: new object[,]
                {
                    { new Guid("2229a2ca-3e77-4637-91e1-06e66630068b"), "[[false,false,false,false,false,false,false,false,false,false],[false,false,false,false,false,false,false,true,false,false],[false,false,false,false,false,false,false,false,true,false],[false,false,true,true,false,false,false,false,true,true],[false,false,true,true,false,false,false,false,false,false],[false,false,false,false,false,false,false,false,false,false],[false,false,false,false,false,false,false,false,false,false],[false,false,false,false,false,false,false,false,false,false],[false,false,false,false,false,false,false,false,false,false],[false,false,false,false,false,false,false,false,false,false]]", "Glider Gun 10x10" },
                    { new Guid("3fedc2a6-9743-4b8c-8087-c34cd0e383ad"), "[[false,false,false],[true,true,true],[false,false,false]]", "Blinker 3x3" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Boards",
                keyColumn: "Id",
                keyValue: new Guid("2229a2ca-3e77-4637-91e1-06e66630068b"));

            migrationBuilder.DeleteData(
                table: "Boards",
                keyColumn: "Id",
                keyValue: new Guid("3fedc2a6-9743-4b8c-8087-c34cd0e383ad"));
        }
    }
}
