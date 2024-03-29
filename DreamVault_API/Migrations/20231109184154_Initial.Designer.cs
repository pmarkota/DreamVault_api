﻿// <auto-generated />
using System;
using DreamVault_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DreamVault_API.Migrations
{
    [DbContext(typeof(PostgresContext))]
    [Migration("20231109184154_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "auth", "aal_level", new[] { "aal1", "aal2", "aal3" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "auth", "code_challenge_method", new[] { "s256", "plain" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "auth", "factor_status", new[] { "unverified", "verified" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "auth", "factor_type", new[] { "totp", "webauthn" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "pgsodium", "key_status", new[] { "default", "valid", "invalid", "expired" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "pgsodium", "key_type", new[] { "aead-ietf", "aead-det", "hmacsha512", "hmacsha256", "auth", "shorthash", "generichash", "kdf", "secretbox", "secretstream", "stream_xchacha20" });
            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "extensions", "pg_stat_statements");
            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "extensions", "pgcrypto");
            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "extensions", "pgjwt");
            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "extensions", "uuid-ossp");
            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "graphql", "pg_graphql");
            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "pgsodium", "pgsodium");
            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "vault", "supabase_vault");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.HasSequence<int>("seq_schema_version", "graphql")
                .IsCyclic();

            modelBuilder.Entity("DreamVault_API.Models.AiDreamImage", b =>
                {
                    b.Property<long>("QueueId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("queue_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("QueueId"));

                    b.Property<long>("AppUserId")
                        .HasColumnType("bigint")
                        .HasColumnName("appUser_id");

                    b.Property<long?>("DreamId")
                        .HasColumnType("bigint")
                        .HasColumnName("dream_id");

                    b.Property<bool?>("ImageGenerated")
                        .HasColumnType("boolean")
                        .HasColumnName("image_generated");

                    b.Property<float?>("ImageGenerationStatus")
                        .HasColumnType("real")
                        .HasColumnName("image_generation_status");

                    b.HasKey("QueueId")
                        .HasName("AiDreamImage_pkey");

                    b.HasIndex("AppUserId");

                    b.HasIndex("DreamId");

                    b.ToTable("AiDreamImages");
                });

            modelBuilder.Entity("DreamVault_API.Models.AppUser", b =>
                {
                    b.Property<long>("AppUserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("appUser_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("AppUserId"));

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<long?>("AiImagesGeneratedThisMonth")
                        .HasColumnType("bigint")
                        .HasColumnName("ai_images_generated_this_month");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("text");

                    b.Property<string>("CustomerId")
                        .HasColumnType("text")
                        .HasColumnName("customer_id");

                    b.Property<string>("Email")
                        .HasColumnType("character varying")
                        .HasColumnName("email");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastResetDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_reset_date");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("text");

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text")
                        .HasColumnName("password_hash");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<DateTime?>("SubscriptionExpirationDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("subscription_expiration_date");

                    b.Property<long?>("SubscriptionPlanId")
                        .HasColumnType("bigint")
                        .HasColumnName("subscription_plan_id");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .HasColumnType("character varying")
                        .HasColumnName("username");

                    b.HasKey("AppUserId")
                        .HasName("AppUsers_pkey");

                    b.HasIndex("SubscriptionPlanId");

                    b.HasIndex(new[] { "Email" }, "AppUsers_email_key")
                        .IsUnique();

                    b.HasIndex(new[] { "Username" }, "AppUsers_username_key")
                        .IsUnique();

                    b.ToTable("AppUsers", t =>
                        {
                            t.HasComment("users table");
                        });
                });

            modelBuilder.Entity("DreamVault_API.Models.Dream", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("AppUserId")
                        .HasColumnType("bigint")
                        .HasColumnName("appUser_id");

                    b.Property<DateTime>("DreamDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("dream_date");

                    b.Property<string>("DreamDescription")
                        .HasColumnType("text")
                        .HasColumnName("dream_description");

                    b.Property<bool?>("DreamPublic")
                        .HasColumnType("boolean")
                        .HasColumnName("dream_public");

                    b.Property<double?>("DreamRating")
                        .HasColumnType("double precision")
                        .HasColumnName("dream_rating");

                    b.Property<string>("DreamTitle")
                        .HasColumnType("character varying")
                        .HasColumnName("dream_title");

                    b.Property<string>("DreamVisualDescription")
                        .HasColumnType("text")
                        .HasColumnName("dream_visual_description");

                    b.HasKey("Id")
                        .HasName("Dreams_pkey");

                    b.HasIndex("AppUserId");

                    b.ToTable("Dreams", t =>
                        {
                            t.HasComment("dreams table");
                        });
                });

            modelBuilder.Entity("DreamVault_API.Models.DreamImage", b =>
                {
                    b.Property<long>("ImageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("image_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("ImageId"));

                    b.Property<long>("DreamId")
                        .HasColumnType("bigint")
                        .HasColumnName("dream_id");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("text")
                        .HasColumnName("image_url");

                    b.HasKey("ImageId")
                        .HasName("DreamImages_pkey");

                    b.HasIndex("DreamId");

                    b.ToTable("DreamImages", t =>
                        {
                            t.HasComment("dream images table");
                        });
                });

            modelBuilder.Entity("DreamVault_API.Models.SharedDream", b =>
                {
                    b.Property<long>("ShareId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("share_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("ShareId"));

                    b.Property<long>("AppUserId")
                        .HasColumnType("bigint")
                        .HasColumnName("appUser_id");

                    b.Property<long>("DreamId")
                        .HasColumnType("bigint")
                        .HasColumnName("dream_id");

                    b.Property<string>("Message")
                        .HasColumnType("text")
                        .HasColumnName("message");

                    b.Property<DateTime>("ShareDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("share_date")
                        .HasDefaultValueSql("now()");

                    b.Property<long>("SharedWithUserId")
                        .HasColumnType("bigint")
                        .HasColumnName("shared_with_user_id");

                    b.HasKey("ShareId")
                        .HasName("SharedDreams_pkey");

                    b.HasIndex("AppUserId");

                    b.HasIndex("DreamId");

                    b.HasIndex("SharedWithUserId");

                    b.ToTable("SharedDreams", t =>
                        {
                            t.HasComment("shared dreams table");
                        });
                });

            modelBuilder.Entity("DreamVault_API.Models.SubscriptionPlan", b =>
                {
                    b.Property<long>("PlanId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("plan_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("PlanId"));

                    b.Property<long>("MonthlyImageLimit")
                        .HasColumnType("bigint")
                        .HasColumnName("monthly_image_limit");

                    b.Property<double>("MonthlyPrice")
                        .HasColumnType("double precision")
                        .HasColumnName("monthly_price");

                    b.Property<string>("PlanName")
                        .IsRequired()
                        .HasColumnType("character varying")
                        .HasColumnName("plan_name");

                    b.HasKey("PlanId")
                        .HasName("SubscriptionPlans_pkey");

                    b.ToTable("SubscriptionPlans", t =>
                        {
                            t.HasComment("subscription plans table");
                        });
                });

            modelBuilder.Entity("DreamVault_API.Models.AiDreamImage", b =>
                {
                    b.HasOne("DreamVault_API.Models.AppUser", "AppUser")
                        .WithMany("AiDreamImages")
                        .HasForeignKey("AppUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("AiDreamImages_appUser_id_fkey");

                    b.HasOne("DreamVault_API.Models.Dream", "Dream")
                        .WithMany("AiDreamImages")
                        .HasForeignKey("DreamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("AiDreamImages_dream_id_fkey");

                    b.Navigation("AppUser");

                    b.Navigation("Dream");
                });

            modelBuilder.Entity("DreamVault_API.Models.AppUser", b =>
                {
                    b.HasOne("DreamVault_API.Models.SubscriptionPlan", "SubscriptionPlan")
                        .WithMany("AppUsers")
                        .HasForeignKey("SubscriptionPlanId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("AppUsers_subscription_plan_id_fkey");

                    b.Navigation("SubscriptionPlan");
                });

            modelBuilder.Entity("DreamVault_API.Models.Dream", b =>
                {
                    b.HasOne("DreamVault_API.Models.AppUser", "AppUser")
                        .WithMany("Dreams")
                        .HasForeignKey("AppUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("Dreams_appUser_id_fkey");

                    b.Navigation("AppUser");
                });

            modelBuilder.Entity("DreamVault_API.Models.DreamImage", b =>
                {
                    b.HasOne("DreamVault_API.Models.Dream", "Dream")
                        .WithMany("DreamImages")
                        .HasForeignKey("DreamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("DreamImages_dream_id_fkey");

                    b.Navigation("Dream");
                });

            modelBuilder.Entity("DreamVault_API.Models.SharedDream", b =>
                {
                    b.HasOne("DreamVault_API.Models.AppUser", "AppUser")
                        .WithMany("SharedDreamAppUsers")
                        .HasForeignKey("AppUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("SharedDreams_appUser_id_fkey");

                    b.HasOne("DreamVault_API.Models.Dream", "Dream")
                        .WithMany("SharedDreams")
                        .HasForeignKey("DreamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("SharedDreams_dream_id_fkey");

                    b.HasOne("DreamVault_API.Models.AppUser", "SharedWithUser")
                        .WithMany("SharedDreamSharedWithUsers")
                        .HasForeignKey("SharedWithUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("SharedDreams_shared_with_user_id_fkey");

                    b.Navigation("AppUser");

                    b.Navigation("Dream");

                    b.Navigation("SharedWithUser");
                });

            modelBuilder.Entity("DreamVault_API.Models.AppUser", b =>
                {
                    b.Navigation("AiDreamImages");

                    b.Navigation("Dreams");

                    b.Navigation("SharedDreamAppUsers");

                    b.Navigation("SharedDreamSharedWithUsers");
                });

            modelBuilder.Entity("DreamVault_API.Models.Dream", b =>
                {
                    b.Navigation("AiDreamImages");

                    b.Navigation("DreamImages");

                    b.Navigation("SharedDreams");
                });

            modelBuilder.Entity("DreamVault_API.Models.SubscriptionPlan", b =>
                {
                    b.Navigation("AppUsers");
                });
#pragma warning restore 612, 618
        }
    }
}
