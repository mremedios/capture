using Capture.Service.Database.JsonHeaders;

namespace Test;

public class QueriesTest
{
    [Test]
    public void AvailableHSelect()
    {
        var repo = new JsonRepository();
        var res = repo.AvailableHeaders();
        Console.WriteLine(res);
    }
    
    [Test]
    public void CustomIdSelect()
    {
        var repo = new JsonRepository();
        var res = repo.Select();
        Console.WriteLine(res);
    }
}