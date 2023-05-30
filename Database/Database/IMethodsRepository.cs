using Database.Models;

namespace Database.Database;

public interface IMethodsRepository
{
    public Task InsertAsync(SipMethods[] methods);

    public void Delete(SipMethods[] methods);
    
    public SipMethods[] FindAll();
}