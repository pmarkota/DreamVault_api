using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DreamVault_API.Models;

public partial class AiDreamImage
{
    [Key]
    [Column("queue_id")]
    public long QueueId { get; set; }

    [Column("appUser_id")]
    public long AppUserId { get; set; }

    [Column("dream_id")]
    public long? DreamId { get; set; }

    [Column("image_generation_status")]
    public float? ImageGenerationStatus { get; set; }

    [Column("image_generated")]
    public bool? ImageGenerated { get; set; }

    [ForeignKey("AppUserId")]
    [InverseProperty("AiDreamImages")]
    public virtual AppUser AppUser { get; set; } = null!;

    [ForeignKey("DreamId")]
    [InverseProperty("AiDreamImages")]
    public virtual Dream? Dream { get; set; }
}
