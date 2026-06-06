using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ActivityService.Api.Extensions;

public static class MigrationExtensions
{
    public static bool ShouldApplyMigrations(this IWebHostEnvironment environment) =>
        environment.IsDevelopment()
        || environment.IsEnvironment("Docker")
        || environment.IsEnvironment("Local")
        || environment.IsEnvironment("LocalDocker");

    public static async Task ApplyMigrationsAsync<TDbContext>(this WebApplication app)
        where TDbContext : DbContext
    {
        const int maxAttempts = 5;
        var delay = TimeSpan.FromSeconds(3);
        Exception? lastException = null;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                using var scope = app.Services.CreateScope();

                var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
                var connectionString = dbContext.Database.GetConnectionString()
                    ?? throw new InvalidOperationException($"Connection string for {typeof(TDbContext).Name} is missing.");

                await EnsureDatabaseExistsAsync(connectionString);
                await dbContext.Database.MigrateAsync();
                return;
            }
            catch (Exception exception) when (attempt < maxAttempts && IsTransient(exception))
            {
                lastException = exception;
                await Task.Delay(delay);
            }
        }

        throw new InvalidOperationException(
            $"Failed to apply migrations for {typeof(TDbContext).Name} after {maxAttempts} attempts.",
            lastException);
    }

    private static bool IsTransient(Exception exception) =>
        exception is NpgsqlException
        || exception is TimeoutException
        || (exception.InnerException is not null && IsTransient(exception.InnerException));

    private static async Task EnsureDatabaseExistsAsync(string connectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        if (string.IsNullOrWhiteSpace(builder.Database))
        {
            throw new InvalidOperationException("Database name is missing from the PostgreSQL connection string.");
        }

        var databaseName = builder.Database;
        builder.Database = "postgres";

        await using var connection = new NpgsqlConnection(builder.ConnectionString);
        await connection.OpenAsync();

        await using var existsCommand = new NpgsqlCommand(
            "SELECT 1 FROM pg_database WHERE datname = @databaseName",
            connection);

        existsCommand.Parameters.AddWithValue("databaseName", databaseName);

        var exists = await existsCommand.ExecuteScalarAsync() is not null;
        if (exists)
        {
            return;
        }

        var safeDatabaseName = databaseName.Replace("\"", "\"\"");

        try
        {
            await using var createCommand = new NpgsqlCommand(
                $"""CREATE DATABASE "{safeDatabaseName}" """,
                connection);

            await createCommand.ExecuteNonQueryAsync();
        }
        catch (PostgresException exception) when (exception.SqlState == PostgresErrorCodes.DuplicateDatabase)
        {
        }
    }
}
