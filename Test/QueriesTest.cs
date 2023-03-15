using Capture.Service.Database.JsonHeaders;
using Newtonsoft.Json;

namespace Test;

public class QueriesTest
{

    [Test]
    public void CustomIdSelect()
    {
        var repo = new JsonRepository();
        var res = repo.FindByHeader("callsessionid", "49ee2f4ad78249e39bab227b02fcd38b");
        Console.WriteLine(res);
    }

    [Test]
    public void kkk()
    {
        var a = "{\"callid\": \"5287826707\",\"callsessionid\": \"49ee2f4ad78249e39bab227b02fcd38b\"}";
        var y = JsonConvert.DeserializeObject<Dictionary<string, string>>(a);
        
    }
}