using tester.Models;
using tester.Repositories;

namespace tester.Services;

public class MeterReadingService : IMeterReadingService
{
    private readonly IMeterReadingRepository _repository;

    public MeterReadingService(IMeterReadingRepository repository)
    {
        _repository = repository;
    }

    public MeterReading RecordReading(string meterId, double value, DateTime timestamp)
    {
        if (string.IsNullOrWhiteSpace(meterId))
            throw new ArgumentException("Meter ID is required.", nameof(meterId));

        // NOTE (practice bug #1): there is no validation on negative values here,
        // even though a meter reading physically can never go negative. Write a
        // test that proves this is currently accepted, then fix the implementation
        // to throw an ArgumentException for negative values.
        var reading = new MeterReading
        {
            MeterId = meterId,
            Value = value,
            Timestamp = timestamp
        };

        return _repository.Add(reading);
    }

    public IEnumerable<MeterReading> GetReadingsForMeter(string meterId) =>
        _repository.GetByMeterId(meterId);

    public IEnumerable<MeterReading> GetAllReadings() =>
        _repository.GetAll();

    public double CalculateAverageConsumption(string meterId)
    {
        var ordered = _repository.GetByMeterId(meterId).OrderBy(r => r.Timestamp).ToList();

        // NOTE (practice bug #2): with 0 or 1 readings, (ordered.Count - 1) is 0 or -1,
        // which makes this division return NaN, Infinity, or a negative-count-based
        // nonsense value instead of failing loudly or returning a sane default.
        // Write a test that exposes this (e.g. call with a meterId that has no
        // readings), then decide what SHOULD happen - return 0? throw? - and fix it.
        double totalConsumption = 0;
        for (int i = 1; i < ordered.Count; i++)
        {
            totalConsumption += ordered[i].Value - ordered[i - 1].Value;
        }

        return totalConsumption / (ordered.Count - 1);
    }
}
