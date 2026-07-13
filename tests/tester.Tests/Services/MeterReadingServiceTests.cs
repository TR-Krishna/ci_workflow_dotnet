using tester.Models;
using tester.Repositories;
using tester.Services;
using Moq;
using Xunit;

namespace tester.Tests.Services;

public class MeterReadingServiceTests
{
    private readonly Mock<IMeterReadingRepository> _repoMock = new();
    private readonly MeterReadingService _sut; // "system under test"

    public MeterReadingServiceTests()
    {
        _sut = new MeterReadingService(_repoMock.Object);
    }

    // ---------- WORKED EXAMPLES ----------

    [Fact]
    public void RecordReading_WithValidData_CallsRepositoryAdd()
    {
        // Arrange
        _repoMock.Setup(r => r.Add(It.IsAny<MeterReading>()))
                 .Returns((MeterReading r) => r);

        // Act
        var result = _sut.RecordReading("MTR-001", 42.5, DateTime.UtcNow);

        // Assert
        Assert.Equal("MTR-001", result.MeterId);
        _repoMock.Verify(r => r.Add(It.Is<MeterReading>(x => x.MeterId == "MTR-001")), Times.Once);
    }

    [Fact]
    public void RecordReading_WithEmptyMeterId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _sut.RecordReading("", 10, DateTime.UtcNow));
    }

    [Fact]
    public void CalculateAverageConsumption_WithTwoReadings_ReturnsDifference()
    {
        // Arrange
        var readings = new List<MeterReading>
        {
            new() { MeterId = "MTR-001", Value = 100, Timestamp = DateTime.UtcNow.AddDays(-1) },
            new() { MeterId = "MTR-001", Value = 150, Timestamp = DateTime.UtcNow }
        };
        _repoMock.Setup(r => r.GetByMeterId("MTR-001")).Returns(readings);

        // Act
        var avg = _sut.CalculateAverageConsumption("MTR-001");

        // Assert
        Assert.Equal(50, avg);
    }

    // ---------- YOUR TURN (see README.md "Practice exercises" for the full list) ----------

    // TODO (practice #1): RecordReading_WithNegativeValue_ShouldThrow
    // Prove the service currently *accepts* a negative reading (physically impossible
    // for a meter). Then go fix MeterReadingService.RecordReading to validate it, and
    // watch this test flip from red to green.

    // TODO (practice #2): CalculateAverageConsumption_WithNoReadings_ShouldNotReturnNaN
    // Call CalculateAverageConsumption for a meterId with zero readings and see what
    // actually comes back today. Decide what the sane behavior should be (0? an
    // exception?), then implement it.

    // TODO (practice #3): CalculateAverageConsumption_WithOneReading_ShouldNotThrow
    // Same idea as #2, but with exactly one reading (edge case: count - 1 == 0).

    // TODO (practice #4): GetReadingsForMeter_DelegatesToRepository
    // Use _repoMock.Verify(...) to confirm the service just passes the call through
    // to the repository, with the right meterId argument.
}
