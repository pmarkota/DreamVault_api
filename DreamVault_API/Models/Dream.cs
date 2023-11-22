using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DreamVault_API.Models;

/// <summary>
/// dreams table
/// </summary>
public partial class Dream
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("appUser_id")]
    public long AppUserId { get; set; }

    [Column("dream_date")]
    public DateTime DreamDate { get; set; }

    [Column("dream_title", TypeName = "character varying")]
    public string? DreamTitle { get; set; }

    [Column("dream_description")]
    public string? DreamDescription { get; set; }

    [Column("dream_rating")]
    public double? DreamRating { get; set; }

    [Column("dream_public")]
    public bool? DreamPublic { get; set; }

    [Column("dream_visual_description")]
    public string? DreamVisualDescription { get; set; }

    [InverseProperty("Dream")]
    public virtual ICollection<AiDreamImage> AiDreamImages { get; set; } = new List<AiDreamImage>();

    [ForeignKey("AppUserId")]
    [InverseProperty("Dreams")]
    public virtual AppUser AppUser { get; set; } = null!;

    [InverseProperty("Dream")]
    public virtual ICollection<DreamImage> DreamImages { get; set; } = new List<DreamImage>();

    [InverseProperty("Dream")]
    public virtual ICollection<SharedDream> SharedDreams { get; set; } = new List<SharedDream>();
}
