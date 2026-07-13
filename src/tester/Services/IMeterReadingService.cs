using tester.Models;

namespace tester.Services;

public interface IMeterReadingService
{
    MeterReading RecordReading(string meterId, double value, DateTime timestamp);
    IEnumerable<MeterReading> GetReadingsForMeter(string meterId);
    IEnumerable<MeterReading> GetAllReadings();
    double CalculateAverageConsumption(string meterId);
}
