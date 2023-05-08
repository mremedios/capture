using System;
using Capture.Service.Models;

namespace Capture.Service.Database;

public interface IReadonlyRepository
{
    public ShortData[] FindByHeader(string header);
    public ShortData[] FindByHeaderWithDate(string header, DateOnly date);
    
    public ShortData[] FindByCallId(string value);
    public ShortData[] FindByCallIdWithDate(string value, DateOnly date);
}