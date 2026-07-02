namespace ABGFileProcessorAPI.Models;

public class FileLog
{
    public string FileName { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Success";
}