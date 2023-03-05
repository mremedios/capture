using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capture.Service.Database.JsonHeaders.Entities;

public class Header
{
    [Key] public DateTime created_date { get; set; }

    public string call_id { get; set; } // todo return key

    public string endpoint { get; set; }

    [Column(TypeName = "jsonb")] 
    public Dictionary<string, string> protocol_header { get; set; }

    public string raw { get; set; }
}