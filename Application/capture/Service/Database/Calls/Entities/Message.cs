using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capture.Service.Database.Calls.Entities;

[Table("messages")]
public class Message
{
    [Key, Column("message_id")] 
    public int MessageId { get; set; }
    
    [Column("protocol_header")] 
    public string Headers { get; set; }
    
    [Column("message")] 
    public string message { get; set; }
    
    [Column("local_call_id")] 
    public int LocalCallId { get; set; }
}