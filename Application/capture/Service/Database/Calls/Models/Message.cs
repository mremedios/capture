using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capture.Service.Database.Calls.Models;

[Table("messages")]
public class Message
{
    [Key, Column("message_id")] 
    public int MessageId { get; set; }
    
    [Column("details")]
    public string Details { get; set; }

    [Column("message")] 
    public string Text { get; set; }
    
    [Column("local_call_id")] 
    public int LocalCallId { get; set; }
}