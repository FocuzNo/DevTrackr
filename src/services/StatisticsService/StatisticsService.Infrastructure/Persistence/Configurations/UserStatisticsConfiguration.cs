using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StatisticsService.Domain.Statistics;

namespace StatisticsService.Infrastructure.Persistence.Configurations;

public sealed class UserStatisticsConfiguration : IEntityTypeConfiguration<UserStatistics>
{
    public void Configure(EntityTypeBuilder<UserStatistics> builder)
    {
        builder.ToTable("user_statistics");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.TotalStudyMinutes).HasColumnName("total_study_minutes");
        builder.Property(x => x.TotalSessions).HasColumnName("total_sessions");
        builder.Property(x => x.CompletedGoals).HasColumnName("completed_goals");
        builder.Property(x => x.ActiveGoals).HasColumnName("active_goals");
        builder.Property(x => x.CurrentStreak).HasColumnName("current_streak");
        builder.Property(x => x.LongestStreak).HasColumnName("longest_streak");
        builder.Property(x => x.AverageDifficulty).HasColumnName("average_difficulty").HasPrecision(5, 2);
        builder.Property(x => x.LastStudyDate).HasColumnName("last_study_date");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(x => x.UserId).IsUnique();
    }
}
