using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthenticationSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DO $$
                BEGIN
                    IF to_regclass('"Users"') IS NOT NULL THEN
                        ALTER TABLE "Users" RENAME TO users;
                    END IF;
                END $$;
                """);

            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS users
                (
                    "Id" uuid NOT NULL,
                    email character varying(200) NOT NULL,
                    normalized_email character varying(200) NOT NULL,
                    password_hash character varying(500) NOT NULL,
                    display_name character varying(100) NOT NULL,
                    created_at timestamp with time zone NOT NULL,
                    updated_at timestamp with time zone NOT NULL,
                    CONSTRAINT "PK_users" PRIMARY KEY ("Id")
                );
                """);

            migrationBuilder.Sql(
                """
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM information_schema.columns
                        WHERE table_name = 'users'
                            AND column_name = 'Email') THEN
                        ALTER TABLE users RENAME COLUMN "Email" TO email;
                    END IF;

                    IF EXISTS (
                        SELECT 1
                        FROM information_schema.columns
                        WHERE table_name = 'users'
                            AND column_name = 'PasswordHash') THEN
                        ALTER TABLE users RENAME COLUMN "PasswordHash" TO password_hash;
                    END IF;

                    IF EXISTS (
                        SELECT 1
                        FROM information_schema.columns
                        WHERE table_name = 'users'
                            AND column_name = 'DisplayName') THEN
                        ALTER TABLE users RENAME COLUMN "DisplayName" TO display_name;
                    END IF;
                END $$;
                """);

            migrationBuilder.Sql(
                """
                ALTER TABLE users
                ADD COLUMN IF NOT EXISTS created_at timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '0001-01-01 00:00:00+00';
                """);

            migrationBuilder.Sql(
                """
                ALTER TABLE users
                ADD COLUMN IF NOT EXISTS normalized_email character varying(200) NOT NULL DEFAULT '';
                """);

            migrationBuilder.Sql(
                """
                ALTER TABLE users
                ADD COLUMN IF NOT EXISTS updated_at timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '0001-01-01 00:00:00+00';
                """);

            migrationBuilder.Sql(
                """
                UPDATE users
                SET normalized_email = UPPER(email)
                WHERE normalized_email = '';
                """);

            migrationBuilder.Sql(
                """
                CREATE UNIQUE INDEX IF NOT EXISTS "IX_users_normalized_email"
                ON users (normalized_email);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""DROP INDEX IF EXISTS "IX_users_normalized_email";""");
            migrationBuilder.Sql("""DROP TABLE IF EXISTS users;""");
        }
    }
}
