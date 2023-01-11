using Capture.Service.Parser;

namespace Test;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void SingleMessage()
    {
        using(Stream source = File.OpenRead("input/sample.bin"))
        {
            var parser = new ParserHePv3(source);
            var x = parser.ParseMessage();
            Console.WriteLine(x);
        }
        Assert.Pass();
    }
    
    [Test]
    public void Stream()
    {
        using(Stream source = File.OpenRead("input/HEP_sample_20221202_095623.bin"))
        {
            var parser = new ParserHePv3(source);
            parser.Parse();
        }
        Assert.Pass();
    }
}