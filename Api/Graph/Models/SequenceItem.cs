namespace Api.Graph.Models;

public record SequenceItem(
    int From,
    int To,
    string Label,
    string At,
    string Text
);