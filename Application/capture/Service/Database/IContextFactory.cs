using Capture.Service.Database.Calls;
using Microsoft.EntityFrameworkCore;

namespace Capture.Service.Database;

public interface IContextFactory
{
    CallsContext CreateContext();
}