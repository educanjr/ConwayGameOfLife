﻿// <auto-generated />
using System;
using ConwayGameOfLife.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ConwayGameOfLife.Data.Migrations
{
    [DbContext(typeof(ConwayDbContext))]
    [Migration("20250317022505_SeddingData")]
    partial class SeddingData
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.18")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ConwayGameOfLife.Application.Entities.Board", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("InitialState")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Boards");

                    b.HasData(
                        new
                        {
                            Id = new Guid("3fedc2a6-9743-4b8c-8087-c34cd0e383ad"),
                            InitialState = "[[false,false,false],[true,true,true],[false,false,false]]",
                            Name = "Blinker 3x3"
                        },
                        new
                        {
                            Id = new Guid("2229a2ca-3e77-4637-91e1-06e66630068b"),
                            InitialState = "[[false,false,false,false,false,false,false,false,false,false],[false,false,false,false,false,false,false,true,false,false],[false,false,false,false,false,false,false,false,true,false],[false,false,true,true,false,false,false,false,true,true],[false,false,true,true,false,false,false,false,false,false],[false,false,false,false,false,false,false,false,false,false],[false,false,false,false,false,false,false,false,false,false],[false,false,false,false,false,false,false,false,false,false],[false,false,false,false,false,false,false,false,false,false],[false,false,false,false,false,false,false,false,false,false]]",
                            Name = "Glider Gun 10x10"
                        });
                });

            modelBuilder.Entity("ConwayGameOfLife.Application.Entities.BoardExecution", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("BoardId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsFinal")
                        .HasColumnType("boolean");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Step")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("BoardId", "Step")
                        .IsUnique();

                    b.ToTable("BoardExecutions");
                });

            modelBuilder.Entity("ConwayGameOfLife.Application.Entities.BoardExecution", b =>
                {
                    b.HasOne("ConwayGameOfLife.Application.Entities.Board", null)
                        .WithMany("Executions")
                        .HasForeignKey("BoardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ConwayGameOfLife.Application.Entities.Board", b =>
                {
                    b.Navigation("Executions");
                });
#pragma warning restore 612, 618
        }
    }
}
