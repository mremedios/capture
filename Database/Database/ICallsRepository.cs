using Database.Models;

namespace Database.Database;

public interface ICallsRepository
{
    public Task InsertRangeAsync(IList<Data> rawMessages);

}