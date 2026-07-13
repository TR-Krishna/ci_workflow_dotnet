using System.Collections.Concurrent;
using tester.Models;

namespace tester.Repositories;

public class InMemoryMeterReadingRepository : IMeterReadingRepository
{
    private readonly ConcurrentDictionary<int, MeterReading> _readings = new();
    private int _nextId = 1;

    public MeterReading Add(MeterReading reading)
    {
        reading.Id = Interlocked.Increment(ref _nextId) - 1;
        _readings[reading.Id] = reading;
        return reading;
    }

    public IEnumerable<MeterReading> GetAll() =>
        _readings.Values.OrderBy(r => r.Timestamp).ToList();

    public IEnumerable<MeterReading> GetByMeterId(string meterId) =>
        _readings.Values.Where(r => r.MeterId == meterId).OrderBy(r => r.Timestamp).ToList();

    public MeterReading? GetById(int id) =>
        _readings.TryGetValue(id, out var reading) ? reading : null;
}
