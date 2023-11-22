using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DreamVault_API.Models;

/// <summary>
/// shared dreams table
/// </summary>
public partial class SharedDream
{
    [Key]
    [Column("share_id")]
    public long ShareId { get; set; }

    [Column("appUser_id")]
    public long AppUserId { get; set; }

    [Column("shared_with_user_id")]
    public long SharedWithUserId { get; set; }

    [Column("dream_id")]
    public long DreamId { get; set; }

    [Column("share_date")]
    public DateTime ShareDate { get; set; }

    [Column("message")]
    public string? Message { get; set; }

    [ForeignKey("AppUserId")]
    [InverseProperty("SharedDreamAppUsers")]
    public virtual AppUser AppUser { get; set; } = null!;

    [ForeignKey("DreamId")]
    [InverseProperty("SharedDreams")]
    public virtual Dream Dream { get; set; } = null!;

    [ForeignKey("SharedWithUserId")]
    [InverseProperty("SharedDreamSharedWithUsers")]
    public virtual AppUser SharedWithUser { get; set; } = null!;
}
