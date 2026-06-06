using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StatisticsService.Domain.Statistics;

namespace StatisticsService.Infrastructure.Persistence.Configurations;

public sealed class DailyStatisticsConfiguration : IEntityTypeConfiguration<DailyStatistics>
{
    public void Configure(EntityTypeBuilder<DailyStatistics> builder)
    {
        builder.ToTable("daily_statistics");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.Date).HasColumnName("date");
        builder.Property(x => x.TotalMinutes).HasColumnName("total_minutes");
        builder.Property(x => x.SessionsCount).HasColumnName("sessions_count");
        builder.Property(x => x.AverageDifficulty).HasColumnName("average_difficulty").HasPrecision(5, 2);
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(x => new { x.UserId, x.Date }).IsUnique();
    }
}
