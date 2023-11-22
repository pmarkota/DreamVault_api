using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DreamVault_API.Models;

/// <summary>
/// subscription plans table
/// </summary>
public partial class SubscriptionPlan
{
    [Key]
    [Column("plan_id")]
    public long PlanId { get; set; }

    [Column("plan_name", TypeName = "character varying")]
    public string PlanName { get; set; } = null!;

    [Column("monthly_image_limit")]
    public long MonthlyImageLimit { get; set; }

    [Column("monthly_price")]
    public double MonthlyPrice { get; set; }

    [InverseProperty("SubscriptionPlan")]
    public virtual ICollection<AppUser> AppUsers { get; set; } = new List<AppUser>();
}
