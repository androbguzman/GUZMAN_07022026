using Microsoft.AspNetCore.Mvc;
using ABGFileProcessorAPI.Services;
using System.Text.Json;

namespace ABGFileProcessorAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly TrackingService _trackingService;

    public FileController(TrackingService trackingService)
    {
        _trackingService = trackingService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Please upload a valid file.");
        }

        // Get file extension to determine the type
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        try
        {
            using var reader = new StreamReader(file.OpenReadStream());
            var content = await reader.ReadToEndAsync();

            if (extension == ".json")
            {
                // 1. Process JSON (Transformation / Parsing)
                using var jsonDoc = JsonDocument.Parse(content);
                
                _trackingService.LogFile(file.FileName, "Success (JSON)");

                return Ok(new 
                { 
                    message = "JSON File successfully validated!", 
                    fileName = file.FileName,
                    formatType = "JSON",
                    timestamp = DateTime.UtcNow 
                });
            }
            else if (extension == ".csv")
            {
                // 2. Process CSV (Calculate Aggregate Average)
                using var stringReader = new StringReader(content);
                var headerLine = await stringReader.ReadLineAsync();
                
                if (headerLine == null)
                {
                    return BadRequest("The uploaded CSV file is empty.");
                }

                var scores = new List<double>();
                string? line;

                while ((line = await stringReader.ReadLineAsync()) != null)
                {
                    var columns = line.Split(',');
                    // Assuming the second column contains numeric values to aggregate
                    if (columns.Length > 1 && double.TryParse(columns[1], out double scoreValue))
                    {
                        scores.Add(scoreValue);
                    }
                }

                double averageScore = scores.Count > 0 ? scores.Average() : 0;

                _trackingService.LogFile(file.FileName, "Success (CSV)");

                return Ok(new 
                { 
                    message = "CSV File successfully processed!", 
                    fileName = file.FileName,
                    formatType = "CSV",
                    totalRowsProcessed = scores.Count,
                    calculatedAverage = averageScore,
                    timestamp = DateTime.UtcNow 
                });
            }
            else
            {
                return BadRequest("Unsupported file format. Please upload a .json or .csv file.");
            }
        }
        catch (JsonException)
        {
            _trackingService.LogFile(file.FileName, "Failed - Invalid JSON structure");
            return BadRequest("The file uploaded is not a valid JSON document.");
        }
        catch (Exception ex)
        {
            _trackingService.LogFile(file.FileName, $"Failed - Error: {ex.Message}");
            return StatusCode(500, "An error occurred while processing your file.");
        }
    }

    [HttpGet("report")]
    public IActionResult GetReport()
    {
        var logs = _trackingService.GetLogs();
        return Ok(new 
        { 
            totalFilesProcessed = logs.Count(), 
            logs = logs 
        });
    }
}