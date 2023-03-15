using System.Data;
using Capture.Service.Database.Calls;
using Capture.Service.Database.Calls.Models;
using Capture.Service.Database;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Test;

public class CallsTest
{
    // [Test]
    // public async Task StoredProcedure()
    // {
    //     var header = new Header
    //     {
    //         HeaderName = "name5",
    //         LocalCallId = 15936,
    //         Value = "value"
    //     };
    //     Header[] arr = { header, header, header };
    //     
    //     var ctx = new CallsContext();
    //
    //     await Capture.Service.Database.Calls.StoredProcedure.CallProcedure(ctx, "insert_data", arr);
    // }
}