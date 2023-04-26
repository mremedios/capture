using System.Threading.Tasks;

namespace Capture.Service.Database;

public interface IPartmanRepository
{
    public Task UpdateRetention(int days);
}