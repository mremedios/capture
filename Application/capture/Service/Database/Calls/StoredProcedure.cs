using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Capture.Service.Database.Calls.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Capture.Service.Database.Calls;

public static class StoredProcedure
{
    public static async Task CallProcedure(CallsContext ctx, string procedureName, Header[] data)
    {
        var connection = (NpgsqlConnection)ctx.Database.GetDbConnection();
        

        using var command = new NpgsqlCommand(procedureName, connection)
        {
            CommandType = CommandType.StoredProcedure,
            Parameters =
            {
                new NpgsqlParameter { Value = data}
            }
        };

        connection.Open();
        await command.ExecuteNonQueryAsync();
    }
}