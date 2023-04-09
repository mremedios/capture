using System.Threading.Tasks;

namespace Capture.Service.Database;

public interface IAvailableHeaderRepository
{
    public Task InsertAsync(string[] headers);

    public void Delete(string[] headers);
    
    public string[] FindAll();
}