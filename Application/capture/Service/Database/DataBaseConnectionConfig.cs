namespace Capture.Service.Database;

public class DataBaseConnectionConfig
{
    public string Address { get; set; }
    public string Database { get; set; }
    // public string Login { get; set; }
    // public string Password { get; set; }
    public string MaxConnections { get; set; }
}