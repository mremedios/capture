using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Models;

namespace Database.Database.Calls.Models;

[Table("methods")]
public class Method
{
    [Key, Column("method")] 
    public SipMethods Value { get; set; }
}