using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DreamVault_API.Models;

public partial class Subscriber
{
    [Key]
    [Column("id")]
    public string Id { get; set; } = null!;

    [Column("customer_id")]
    public string? CustomerId { get; set; }

    [Column("status")]
    public string? Status { get; set; }

    [Column("current_period_end")]
    public string? CurrentPeriodEnd { get; set; }
}
