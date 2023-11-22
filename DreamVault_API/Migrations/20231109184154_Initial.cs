using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DreamVault_API.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "graphql");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:auth.aal_level", "aal1,aal2,aal3")
                .Annotation("Npgsql:Enum:auth.code_challenge_method", "s256,plain")
                .Annotation("Npgsql:Enum:auth.factor_status", "unverified,verified")
                .Annotation("Npgsql:Enum:auth.factor_type", "totp,webauthn")
                .Annotation("Npgsql:Enum:pgsodium.key_status", "default,valid,invalid,expired")
                .Annotation("Npgsql:Enum:pgsodium.key_type", "aead-ietf,aead-det,hmacsha512,hmacsha256,auth,shorthash,generichash,kdf,secretbox,secretstream,stream_xchacha20")
                .Annotation("Npgsql:PostgresExtension:extensions.pg_stat_statements", ",,")
                .Annotation("Npgsql:PostgresExtension:extensions.pgcrypto", ",,")
                .Annotation("Npgsql:PostgresExtension:extensions.pgjwt", ",,")
                .Annotation("Npgsql:PostgresExtension:extensions.uuid-ossp", ",,")
                .Annotation("Npgsql:PostgresExtension:graphql.pg_graphql", ",,")
                .Annotation("Npgsql:PostgresExtension:pgsodium.pgsodium", ",,")
                .Annotation("Npgsql:PostgresExtension:vault.supabase_vault", ",,");

            migrationBuilder.CreateSequence<int>(
                name: "seq_schema_version",
                schema: "graphql",
                cyclic: true);

            migrationBuilder.CreateTable(
                name: "SubscriptionPlans",
                columns: table => new
                {
                    plan_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    plan_name = table.Column<string>(type: "character varying", nullable: false),
                    monthly_image_limit = table.Column<long>(type: "bigint", nullable: false),
                    monthly_price = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SubscriptionPlans_pkey", x => x.plan_id);
                },
                comment: "subscription plans table");

            migrationBuilder.CreateTable(
                name: "AppUsers",
                columns: table => new
                {
                    appUser_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    username = table.Column<string>(type: "character varying", nullable: true),
                    email = table.Column<string>(type: "character varying", nullable: true),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    subscription_expiration_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ai_images_generated_this_month = table.Column<long>(type: "bigint", nullable: true),
                    last_reset_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    subscription_plan_id = table.Column<long>(type: "bigint", nullable: true),
                    customer_id = table.Column<string>(type: "text", nullable: true),
                    Id = table.Column<string>(type: "text", nullable: true),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "text", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "text", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("AppUsers_pkey", x => x.appUser_id);
                    table.ForeignKey(
                        name: "AppUsers_subscription_plan_id_fkey",
                        column: x => x.subscription_plan_id,
                        principalTable: "SubscriptionPlans",
                        principalColumn: "plan_id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "users table");

            migrationBuilder.CreateTable(
                name: "Dreams",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    appUser_id = table.Column<long>(type: "bigint", nullable: false),
                    dream_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dream_title = table.Column<string>(type: "character varying", nullable: true),
                    dream_description = table.Column<string>(type: "text", nullable: true),
                    dream_rating = table.Column<double>(type: "double precision", nullable: true),
                    dream_public = table.Column<bool>(type: "boolean", nullable: true),
                    dream_visual_description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Dreams_pkey", x => x.id);
                    table.ForeignKey(
                        name: "Dreams_appUser_id_fkey",
                        column: x => x.appUser_id,
                        principalTable: "AppUsers",
                        principalColumn: "appUser_id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "dreams table");

            migrationBuilder.CreateTable(
                name: "AiDreamImages",
                columns: table => new
                {
                    queue_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    appUser_id = table.Column<long>(type: "bigint", nullable: false),
                    dream_id = table.Column<long>(type: "bigint", nullable: true),
                    image_generation_status = table.Column<float>(type: "real", nullable: true),
                    image_generated = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("AiDreamImage_pkey", x => x.queue_id);
                    table.ForeignKey(
                        name: "AiDreamImages_appUser_id_fkey",
                        column: x => x.appUser_id,
                        principalTable: "AppUsers",
                        principalColumn: "appUser_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "AiDreamImages_dream_id_fkey",
                        column: x => x.dream_id,
                        principalTable: "Dreams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DreamImages",
                columns: table => new
                {
                    image_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    dream_id = table.Column<long>(type: "bigint", nullable: false),
                    image_url = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("DreamImages_pkey", x => x.image_id);
                    table.ForeignKey(
                        name: "DreamImages_dream_id_fkey",
                        column: x => x.dream_id,
                        principalTable: "Dreams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "dream images table");

            migrationBuilder.CreateTable(
                name: "SharedDreams",
                columns: table => new
                {
                    share_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    appUser_id = table.Column<long>(type: "bigint", nullable: false),
                    shared_with_user_id = table.Column<long>(type: "bigint", nullable: false),
                    dream_id = table.Column<long>(type: "bigint", nullable: false),
                    share_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    message = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SharedDreams_pkey", x => x.share_id);
                    table.ForeignKey(
                        name: "SharedDreams_appUser_id_fkey",
                        column: x => x.appUser_id,
                        principalTable: "AppUsers",
                        principalColumn: "appUser_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "SharedDreams_dream_id_fkey",
                        column: x => x.dream_id,
                        principalTable: "Dreams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "SharedDreams_shared_with_user_id_fkey",
                        column: x => x.shared_with_user_id,
                        principalTable: "AppUsers",
                        principalColumn: "appUser_id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "shared dreams table");

            migrationBuilder.CreateIndex(
                name: "IX_AiDreamImages_appUser_id",
                table: "AiDreamImages",
                column: "appUser_id");

            migrationBuilder.CreateIndex(
                name: "IX_AiDreamImages_dream_id",
                table: "AiDreamImages",
                column: "dream_id");

            migrationBuilder.CreateIndex(
                name: "AppUsers_email_key",
                table: "AppUsers",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "AppUsers_username_key",
                table: "AppUsers",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_subscription_plan_id",
                table: "AppUsers",
                column: "subscription_plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_DreamImages_dream_id",
                table: "DreamImages",
                column: "dream_id");

            migrationBuilder.CreateIndex(
                name: "IX_Dreams_appUser_id",
                table: "Dreams",
                column: "appUser_id");

            migrationBuilder.CreateIndex(
                name: "IX_SharedDreams_appUser_id",
                table: "SharedDreams",
                column: "appUser_id");

            migrationBuilder.CreateIndex(
                name: "IX_SharedDreams_dream_id",
                table: "SharedDreams",
                column: "dream_id");

            migrationBuilder.CreateIndex(
                name: "IX_SharedDreams_shared_with_user_id",
                table: "SharedDreams",
                column: "shared_with_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AiDreamImages");

            migrationBuilder.DropTable(
                name: "DreamImages");

            migrationBuilder.DropTable(
                name: "SharedDreams");

            migrationBuilder.DropTable(
                name: "Dreams");

            migrationBuilder.DropTable(
                name: "AppUsers");

            migrationBuilder.DropTable(
                name: "SubscriptionPlans");

            migrationBuilder.DropSequence(
                name: "seq_schema_version",
                schema: "graphql");
        }
    }
}
