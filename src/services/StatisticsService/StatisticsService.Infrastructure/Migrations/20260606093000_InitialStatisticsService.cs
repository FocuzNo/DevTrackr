using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StatisticsService.Infrastructure.Migrations;

public partial class InitialStatisticsService : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Statistics",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                TotalSessions = table.Column<int>(type: "integer", nullable: false),
                TotalMinutes = table.Column<int>(type: "integer", nullable: false),
                CompletedGoals = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Statistics", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Statistics");
    }
}
