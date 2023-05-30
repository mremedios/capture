namespace Database.Database;

public interface IPartmanRepository
{
    public Task UpdateRetention(int days);
}