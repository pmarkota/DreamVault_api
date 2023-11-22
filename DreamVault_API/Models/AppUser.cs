using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DreamVault_API.Models;

/// <summary>
/// users table
/// </summary>
[Index("Email", Name = "AppUsers_email_key", IsUnique = true)]
[Index("Username", Name = "AppUsers_username_key", IsUnique = true)]
public partial class AppUser
{
    [Key]
    [Column("appUser_id")]
    public long AppUserId { get; set; }

    [Column("username", TypeName = "character varying")]
    public string? Username { get; set; }

    [Column("email", TypeName = "character varying")]
    public string? Email { get; set; }

    [Column("password_hash")]
    public string? PasswordHash { get; set; }

    [Column("subscription_expiration_date")]
    public DateTime? SubscriptionExpirationDate { get; set; }

    [Column("ai_images_generated_this_month")]
    public long? AiImagesGeneratedThisMonth { get; set; }

    [Column("last_reset_date")]
    public DateTime? LastResetDate { get; set; }

    [Column("subscription_plan_id")]
    public long? SubscriptionPlanId { get; set; }

    [Column("customer_id")]
    public string? CustomerId { get; set; }

    [InverseProperty("AppUser")]
    public virtual ICollection<AiDreamImage> AiDreamImages { get; set; } = new List<AiDreamImage>();

    [InverseProperty("AppUser")]
    public virtual ICollection<Dream> Dreams { get; set; } = new List<Dream>();

    [InverseProperty("AppUser")]
    public virtual ICollection<SharedDream> SharedDreamAppUsers { get; set; } = new List<SharedDream>();

    [InverseProperty("SharedWithUser")]
    public virtual ICollection<SharedDream> SharedDreamSharedWithUsers { get; set; } = new List<SharedDream>();

    [ForeignKey("SubscriptionPlanId")]
    [InverseProperty("AppUsers")]
    public virtual SubscriptionPlan? SubscriptionPlan { get; set; }
}
