using Database.Database.Calls;

namespace Database.Database;

public interface IContextFactory
{
    CallsContext CreateContext();
}