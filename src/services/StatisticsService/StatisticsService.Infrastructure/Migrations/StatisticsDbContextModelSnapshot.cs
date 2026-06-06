using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using StatisticsService.Infrastructure.Persistence;

namespace StatisticsService.Infrastructure.Migrations;

[DbContext(typeof(StatisticsDbContext))]
partial class StatisticsDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "10.0.6")
            .HasAnnotation("Relational:MaxIdentifierLength", 63);

        modelBuilder.Entity("StatisticsService.Domain.Statistics.StudyStatistics", b =>
        {
            b.Property<Guid>("Id")
                .ValueGeneratedNever()
                .HasColumnType("uuid");

            b.Property<int>("CompletedGoals")
                .HasColumnType("integer");

            b.Property<int>("TotalMinutes")
                .HasColumnType("integer");

            b.Property<int>("TotalSessions")
                .HasColumnType("integer");

            b.Property<Guid>("UserId")
                .HasColumnType("uuid");

            b.HasKey("Id");

            b.ToTable("Statistics", (string?)null);
        });
#pragma warning restore 612, 618
    }
}
