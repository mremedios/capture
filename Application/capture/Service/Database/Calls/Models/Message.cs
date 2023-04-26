using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Capture.Service.Database.Calls.Models;

[Table("messages"), PrimaryKey(nameof(Details), nameof(LocalCallId))]
public class Message
{
    [Column("at")] public DateOnly Date { get; set; }

    [Column("details")] public string Details { get; set; }

    [Column("message")] public string Text { get; set; }

    [Column("local_call_id")] public int LocalCallId { get; set; }
}