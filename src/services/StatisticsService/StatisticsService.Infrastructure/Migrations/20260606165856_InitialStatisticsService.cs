using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StatisticsService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialStatisticsService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "daily_statistics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    total_minutes = table.Column<int>(type: "integer", nullable: false),
                    sessions_count = table.Column<int>(type: "integer", nullable: false),
                    average_difficulty = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_daily_statistics", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "processed_integration_events",
                columns: table => new
                {
                    event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_type = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    processed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_processed_integration_events", x => x.event_id);
                });

            migrationBuilder.CreateTable(
                name: "topic_statistics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    topic = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    total_minutes = table.Column<int>(type: "integer", nullable: false),
                    sessions_count = table.Column<int>(type: "integer", nullable: false),
                    average_difficulty = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_topic_statistics", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_statistics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    total_study_minutes = table.Column<int>(type: "integer", nullable: false),
                    total_sessions = table.Column<int>(type: "integer", nullable: false),
                    completed_goals = table.Column<int>(type: "integer", nullable: false),
                    active_goals = table.Column<int>(type: "integer", nullable: false),
                    current_streak = table.Column<int>(type: "integer", nullable: false),
                    longest_streak = table.Column<int>(type: "integer", nullable: false),
                    average_difficulty = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    last_study_date = table.Column<DateOnly>(type: "date", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_statistics", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_daily_statistics_user_id_date",
                table: "daily_statistics",
                columns: new[] { "user_id", "date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_topic_statistics_user_id_topic",
                table: "topic_statistics",
                columns: new[] { "user_id", "topic" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_statistics_user_id",
                table: "user_statistics",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "daily_statistics");

            migrationBuilder.DropTable(
                name: "processed_integration_events");

            migrationBuilder.DropTable(
                name: "topic_statistics");

            migrationBuilder.DropTable(
                name: "user_statistics");
        }
    }
}
