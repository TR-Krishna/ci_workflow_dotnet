using tester.Controllers;
using tester.Models;
using tester.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace tester.Tests.Controllers;

public class MeterReadingsControllerTests
{
    private readonly Mock<IMeterReadingService> _serviceMock = new();
    private readonly MeterReadingsController _sut;

    public MeterReadingsControllerTests()
    {
        _sut = new MeterReadingsController(_serviceMock.Object);
    }

    // ---------- WORKED EXAMPLES ----------

    [Fact]
    public void GetAll_ReturnsOkWithReadings()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetAllReadings())
                    .Returns(new List<MeterReading> { new() { MeterId = "MTR-001", Value = 10 } });

        // Act
        var result = _sut.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var readings = Assert.IsAssignableFrom<IEnumerable<MeterReading>>(okResult.Value);
        Assert.Single(readings);
    }

    [Fact]
    public void Post_WithInvalidMeterId_ReturnsBadRequest()
    {
        // Arrange
        _serviceMock.Setup(s => s.RecordReading("", 10, It.IsAny<DateTime>()))
                    .Throws(new ArgumentException("Meter ID is required."));

        // Act
        var result = _sut.Post(new RecordReadingRequest("", 10, DateTime.UtcNow));

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    // ---------- YOUR TURN ----------

    // TODO (practice #5): Post_WithValidRequest_ReturnsCreatedAtAction
    // Mock the service to return a MeterReading, call Post, and assert the result
    // is a CreatedAtActionResult (not just 200 OK - POST that creates something
    // should return 201 Created with a Location header).

    // TODO (practice #6): GetAverageConsumption_ReturnsOkWithValue
    // Mock IMeterReadingService.CalculateAverageConsumption to return a known
    // value, call the controller action, and assert the OK result wraps it.
}
