using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Database.Calls.Models;

[Table("available_headers")]
public class AvailableHeader
{
	[Key, Column("header")]
	public string Header { get; set; }
}