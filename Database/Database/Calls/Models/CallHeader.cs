using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Database.Database.Calls.Models;

[PrimaryKey(nameof(Value), nameof(LocalCallId)), Table("headers")]
public class CallHeader
{
    [Column("header")] public string Header { get; set; }

    [Column("at")] public DateOnly At { get; set; }

    [Column("value")] public string Value { get; set; }

    [Column("local_call_id")] public int LocalCallId { get; set; }
}