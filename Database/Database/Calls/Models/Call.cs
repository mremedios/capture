using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Database.Calls.Models;

[Table("calls")]
public class Call
{
	[DatabaseGenerated(DatabaseGeneratedOption.Identity), Key, Column("local_call_id")]
	public int LocalCallId { get; set; }

	[Column("at")]
	public DateTime Date { get; set; }

	[Column("host")]
	public string Host { get; set; }

	[Column("call_id")]
	public string CallId { get; set; }

}