using System.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Database.Database.Calls;

public static class ContextExtension
{
    public static async Task StoredProcedure<T>(this CallsContext ctx, string procedureName, T data)
    {
        var connection = (NpgsqlConnection)ctx.Database.GetDbConnection();
        
        using var command = new NpgsqlCommand(procedureName, connection)
        {
            CommandType = CommandType.StoredProcedure,
            Parameters =
            {
                new NpgsqlParameter { Value = data }
            }
        };

        connection.Open();
        await command.ExecuteNonQueryAsync();
    }
}