using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Database.Calls.Models;

[Table("messages")]
public class Message
{
    [Column("message_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity), Key] public int Id { get; set; }
    
    [Column("at")] public DateOnly Date { get; set; }

    [Column("details")] public string Details { get; set; }

    [Column("message")] public string Text { get; set; }

    [Column("local_call_id")] public int LocalCallId { get; set; }
}