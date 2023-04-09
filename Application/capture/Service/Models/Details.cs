namespace Capture.Service.Models;

public record Details(
    string Source,
    string Destination,
    uint TimeUnix,
    uint TimeOffset
);