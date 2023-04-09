namespace Api.Graph.Models;

public class Sequence
{
    public IEnumerable<string> Endpoints { get; set; } = new List<string>();
    public IList<SequenceItem> Messages { get; set; } = new List<SequenceItem>();
}