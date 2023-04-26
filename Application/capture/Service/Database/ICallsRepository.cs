using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Capture.Service.Database.Calls.Models;
using Capture.Service.Handler;
using Capture.Service.Models;

namespace Capture.Service.Database;

public interface ICallsRepository
{
    public Task InsertRangeAsync(IList<Data> rawMessages);

    public ShortData[] FindByHeader(string header);
    
    public ShortData[] FindByHeaderAndDate(string header, DateOnly date);
}