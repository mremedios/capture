using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capture.Service.Database.JsonHeaders.Entities;


[Table("available_headers")]
public class AvailableHeader
{
    [Key, Column("header")]
    public string Header { get; set; }
}