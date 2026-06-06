using System;
using ActivityService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ActivityService.Infrastructure.Migrations;

[DbContext(typeof(ActivityDbContext))]
partial class ActivityDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "10.0.6")
            .HasAnnotation("Relational:MaxIdentifierLength", 63);

        modelBuilder.Entity("ActivityService.Domain.Sessions.StudySession", b =>
        {
            b.Property<Guid>("Id")
                .ValueGeneratedNever()
                .HasColumnType("uuid");

            b.Property<int>("DurationMinutes")
                .HasColumnType("integer");

            b.Property<Guid>("GoalId")
                .HasColumnType("uuid");

            b.Property<DateOnly>("SessionDate")
                .HasColumnType("date");

            b.Property<string>("Topic")
                .IsRequired()
                .HasMaxLength(150)
                .HasColumnType("character varying(150)");

            b.Property<Guid>("UserId")
                .HasColumnType("uuid");

            b.HasKey("Id");

            b.ToTable("StudySessions", (string?)null);
        });
#pragma warning restore 612, 618
    }
}
