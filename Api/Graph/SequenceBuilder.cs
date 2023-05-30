using Api.Graph.Models;
using Database.Models;

namespace Api.Graph;

public class GraphBuilder
{
    private List<string> _endpoints = new();
    private Dictionary<string, int> _endpointIndex = new();
    private IEnumerable<ShortData> _messages;

    public GraphBuilder(ShortData[] messages)
    {
        _messages = messages.OrderBy(x => x.Details.TimeUnix).ThenBy(x => x.Details.TimeOffset);
    }

    public Sequence Build()
    {
        var seq = new Sequence();

        foreach (var (text, details) in _messages)
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