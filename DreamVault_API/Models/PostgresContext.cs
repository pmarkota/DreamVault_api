using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DreamVault_API.Models;

public partial class PostgresContext : DbContext
{
    public PostgresContext()
    {
    }

    public PostgresContext(DbContextOptions<PostgresContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AiDreamImage> AiDreamImages { get; set; }

    public virtual DbSet<AppUser> AppUsers { get; set; }

    public virtual DbSet<Dream> Dreams { get; set; }

    public virtual DbSet<DreamImage> DreamImages { get; set; }

    public virtual DbSet<SharedDream> SharedDreams { get; set; }

    public virtual DbSet<Subscriber> Subscribers { get; set; }

    public virtual DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("User Id=postgres;Password=Qk2NKYcHc2VhMIPs;Server=db.bjzqexdidzsoyqgpyikb.supabase.co;Port=5432;Database=postgres");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("auth", "aal_level", new[] { "aal1", "aal2", "aal3" })
            .HasPostgresEnum("auth", "code_challenge_method", new[] { "s256", "plain" })
            .HasPostgresEnum("auth", "factor_status", new[] { "unverified", "verified" })
            .HasPostgresEnum("auth", "factor_type", new[] { "totp", "webauthn" })
            .HasPostgresEnum("pgsodium", "key_status", new[] { "default", "valid", "invalid", "expired" })
            .HasPostgresEnum("pgsodium", "key_type", new[] { "aead-ietf", "aead-det", "hmacsha512", "hmacsha256", "auth", "shorthash", "generichash", "kdf", "secretbox", "secretstream", "stream_xchacha20" })
            .HasPostgresExtension("extensions", "pg_stat_statements")
            .HasPostgresExtension("extensions", "pgcrypto")
            .HasPostgresExtension("extensions", "pgjwt")
            .HasPostgresExtension("extensions", "uuid-ossp")
            .HasPostgresExtension("graphql", "pg_graphql")
            .HasPostgresExtension("pgsodium", "pgsodium")
            .HasPostgresExtension("vault", "supabase_vault");

        modelBuilder.Entity<AiDreamImage>(entity =>
        {
            entity.HasKey(e => e.QueueId).HasName("AiDreamImage_pkey");

            entity.HasOne(d => d.AppUser).WithMany(p => p.AiDreamImages).HasConstraintName("AiDreamImages_appUser_id_fkey");

            entity.HasOne(d => d.Dream).WithMany(p => p.AiDreamImages)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("AiDreamImages_dream_id_fkey");
        });

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(e => e.AppUserId).HasName("AppUsers_pkey");

            entity.ToTable(tb => tb.HasComment("users table"));

            entity.HasOne(d => d.SubscriptionPlan).WithMany(p => p.AppUsers)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("AppUsers_subscription_plan_id_fkey");
        });

        modelBuilder.Entity<Dream>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Dreams_pkey");

            entity.ToTable(tb => tb.HasComment("dreams table"));

            entity.HasOne(d => d.AppUser).WithMany(p => p.Dreams).HasConstraintName("Dreams_appUser_id_fkey");
        });

        modelBuilder.Entity<DreamImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("DreamImages_pkey");

            entity.ToTable(tb => tb.HasComment("dream images table"));

            entity.HasOne(d => d.Dream).WithMany(p => p.DreamImages).HasConstraintName("DreamImages_dream_id_fkey");
        });

        modelBuilder.Entity<SharedDream>(entity =>
        {
            entity.HasKey(e => e.ShareId).HasName("SharedDreams_pkey");

            entity.ToTable(tb => tb.HasComment("shared dreams table"));

            entity.Property(e => e.ShareDate).HasDefaultValueSql("now()");

            entity.HasOne(d => d.AppUser).WithMany(p => p.SharedDreamAppUsers).HasConstraintName("SharedDreams_appUser_id_fkey");

            entity.HasOne(d => d.Dream).WithMany(p => p.SharedDreams).HasConstraintName("SharedDreams_dream_id_fkey");

            entity.HasOne(d => d.SharedWithUser).WithMany(p => p.SharedDreamSharedWithUsers).HasConstraintName("SharedDreams_shared_with_user_id_fkey");
        });

        modelBuilder.Entity<Subscriber>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Subsribers_pkey");
        });

        modelBuilder.Entity<SubscriptionPlan>(entity =>
        {
            entity.HasKey(e => e.PlanId).HasName("SubscriptionPlans_pkey");

            entity.ToTable(tb => tb.HasComment("subscription plans table"));
        });
        modelBuilder.HasSequence<int>("seq_schema_version", "graphql").IsCyclic();

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
