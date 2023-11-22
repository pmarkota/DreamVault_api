using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DreamVault_API.Models;

/// <summary>
/// dream images table
/// </summary>
public partial class DreamImage
{
    [Key]
    [Column("image_id")]
    public long ImageId { get; set; }

    [Column("dream_id")]
    public long DreamId { get; set; }

    [Column("image_url")]
    public string? ImageUrl { get; set; }

    [ForeignKey("DreamId")]
    [InverseProperty("DreamImages")]
    public virtual Dream Dream { get; set; } = null!;
}
