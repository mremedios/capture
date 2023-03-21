using Capture.Service.Parser;

namespace Test;

public class Tests
{

    [Test]
    public void SimpleMessage()
    {

        var input = File.ReadAllBytes("input/sample.bin");
        
        var h = ParserHePv3.ParseMessage(input);

        Assert.That(h.Hep.destinationIPAddress, Is.EqualTo("10.232.35.73"));
        Assert.That(h.Hep.destinationPort, Is.EqualTo(5060));
    }
    
    [Test]
    public void MessageWithHeaders()
    {
        var input = File.ReadAllBytes("input/in1.bin");
        var h = ParserHePv3.ParseMessage(input);

        Assert.That(h.Hep.destinationIPAddress, Is.EqualTo("10.232.35.73"));
        Assert.That(h.Hep.destinationPort, Is.EqualTo(5060));
    }
}