using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Capture.Service.Models;

namespace Capture.Service.Database;

public interface ICallsRepository
{
    public Task InsertRangeAsync(IList<Data> rawMessages);

}