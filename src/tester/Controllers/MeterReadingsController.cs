using tester.Models;
using tester.Services;
using Microsoft.AspNetCore.Mvc;

namespace tester.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MeterReadingsController : ControllerBase
{
    private readonly IMeterReadingService _service;

    public MeterReadingsController(IMeterReadingService service)
    {
        _service = service;
    }

    [HttpGet]
    public ActionResult<IEnumerable<MeterReading>> GetAll() =>
        Ok(_service.GetAllReadings());

    [HttpGet("{meterId}")]
    public ActionResult<IEnumerable<MeterReading>> GetByMeterId(string meterId) =>
        Ok(_service.GetReadingsForMeter(meterId));

    [HttpGet("{meterId}/average-consumption")]
    public ActionResult<double> GetAverageConsumption(string meterId) =>
        Ok(_service.CalculateAverageConsumption(meterId));

    [HttpPost]
    public ActionResult<MeterReading> Post([FromBody] RecordReadingRequest request)
    {
        if (request is null)
            return BadRequest();

        try
        {
            var reading = _service.RecordReading(request.MeterId, request.Value, request.Timestamp);
            return CreatedAtAction(nameof(GetByMeterId), new { meterId = reading.MeterId }, reading);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

public record RecordReadingRequest(string MeterId, double Value, DateTime Timestamp);
