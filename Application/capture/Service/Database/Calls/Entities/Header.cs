namespace Capture.Service.Database.Calls.Entities;

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[PrimaryKey(nameof(Value), nameof(LocalCallId)),
 Table("headers")]
public class Header
{
    public string header { get; set; }
    
    [Column("value")] 
    public string Value { get; set; }
    
    [Column("local_call_id")] 
    public int LocalCallId { get; set; }
}