using Api.Graph.Models;
using Database.Models;

namespace Api.Graph;

public class GraphBuilder
{
    private readonly List<string> _endpoints = new();
    private readonly Dictionary<string, int> _endpointIndex = new();
    private readonly IList<IEnumerable<ShortData>> _calls = new List<IEnumerable<ShortData>>();

    public GraphBuilder(ShortData[] messages)
    {
        var calls = messages.ToList().GroupBy(m => m.CallId);
        foreach (var grouping in calls)
        {
            var x = grouping.OrderBy(x => x.Details.TimeUnix).ThenBy(x => x.Details.TimeOffset);
            _calls.Add(x);
        }
    }

    public IEnumerable<Sequence> Build()
    {
        return _calls.Select(BuildSequence);
    }

    private Sequence BuildSequence(IEnumerable<ShortData> list)
    {
        var seq = new Sequence();

        foreach (var (text, details, _) in list)
        {
            var label = text.Split(new[] { '\r', '\n' }, 2).FirstOrDefault("");

            var time = DateTimeOffset.FromUnixTimeSeconds(details.TimeUnix);
            var at = $"{time.ToString("h:mm:ss")}.{details.TimeOffset}";

            var item = new SequenceItem(
                GetIndex(details.Source),
                GetIndex(details.Destination),
                label,
                at,
                text
            );

            seq.Messages.Add(item);
        }

        seq.Endpoints = _endpoints;

        return seq;
    }

    private int GetIndex(string key)
    {
        if (!_endpointIndex.ContainsKey(key))
        {
            _endpoints.Add(key);
            _endpointIndex[key] = _endpoints.Count - 1;
        }

        return _endpointIndex[key];
    }
}