using ABGFileProcessorAPI.Models;
using System.Collections.Concurrent;

namespace ABGFileProcessorAPI.Services;

public class TrackingService
{
    private readonly ConcurrentBag<FileLog> _logs = new();

    public void LogFile(string fileName, string status)
    {
        _logs.Add(new FileLog { FileName = fileName, Status = status });
    }

    public IEnumerable<FileLog> GetLogs() => _logs.ToList();
}