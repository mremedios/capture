using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Test
{
    
    public class TheEasiestBenchmark
    {
        [Benchmark(Description = "Summ100")]
        public int Test100()
        {
            return Enumerable.Range(1, 100).Sum();
        }

        [Benchmark(Description = "Summ200")]
        public int Test200()
        {
            return Enumerable.Range(1, 200).Sum();
        }
    }
    
    
    public class Program
    {
        public static void Main()
        {            
            BenchmarkRunner.Run<TheEasiestBenchmark>();
        }
    }
}