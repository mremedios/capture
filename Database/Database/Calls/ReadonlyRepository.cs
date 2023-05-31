using System.Text.Json;
using Database.Database.Calls.Models;
using Database.Models;

namespace Database.Database.Calls;

public class ReadonlyRepository : IReadonlyRepository
{
    private readonly IContextFactory _contextFactory;

    public ReadonlyRepository(IContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    private ShortData[] FromQuery(IQueryable<Message> messages)
    {
        var shortMsg = messages.ToList().Select(m =>
            new ShortData(
                m.Text,
                JsonSerializer.Deserialize<Details>(m.Details),
                m.LocalCallId
            )
        );

        return shortMsg.ToArray();
    }

    public ShortData[] FindByHeader(string header)
    {
        using var ctx = _contextFactory.CreateContext();
        var messages =
            ctx.Headers
                .Where(h => h.Value == header)
                .SelectMany(h =>
                        ctx.Messages
                            .Where(m => h.LocalCallId == m.LocalCallId && h.At == m.Date)
                            .DefaultIfEmpty(),
                    (h, m) => m);

        return FromQuery(messages);
    }

    public ShortData[] FindByHeaderWithDate(string header, DateOnly date)
    {
        using var ctx = _contextFactory.CreateContext();
        var messages =
            ctx.Headers
                .Where(h => h.Value == header && h.At == date)
                .SelectMany(h =>
                        ctx.Messages
                            .Where(m => h.LocalCallId == m.LocalCallId && h.At == m.Date)
                            .DefaultIfEmpty(),
                    (h, m) => m);

        return FromQuery(messages);
    }

    public ShortData[] FindByCallId(string value)
    {
        using var ctx = _contextFactory.CreateContext();
        var messages =
            ctx.Calls
                .Where(c => c.CallId == value)
                .SelectMany(c =>
                        ctx.Messages
                            .Where(m => c.LocalCallId == m.LocalCallId && DateOnly.FromDateTime(c.Date) == m.Date)
                            .DefaultIfEmpty(),
                    (с, m) => m);

        return FromQuery(messages);
    }

    public ShortData[] FindByCallIdWithDate(string value, DateOnly date)
    {
        using var ctx = _contextFactory.CreateContext();
        var messages =
            ctx.Calls
                .Where(c => c.CallId == value && DateOnly.FromDateTime(c.Date) == date)
                .SelectMany(c =>
                        ctx.Messages
                            .Where(m => c.LocalCallId == m.LocalCallId && DateOnly.FromDateTime(c.Date) == m.Date)
                            .DefaultIfEmpty(),
                    (с, m) => m);

        return FromQuery(messages);
    }
}