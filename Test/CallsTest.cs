using System.Data;
using Capture.Service.Database.Calls;
using Capture.Service.Database.Calls.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Test;

public class CallsTest
{
    [Test]
    public void Procedure()
    {
        var ctx = new CallsContext();
        var header = new Header
        {
            header = "name",
            LocalCallId = 13179,
            Value = "value"
        };
        Header[] arr = { header, header, header };

        NpgsqlConnection x = (NpgsqlConnection) ctx.Database.GetDbConnection();
        
        x.Open();
        using var command = new NpgsqlCommand("insert_data", x)
        {
            CommandType = CommandType.StoredProcedure,
            Parameters =
            {
                new() { Value = arr}
            }
        };

        command.ExecuteNonQuery();
        
    }

}