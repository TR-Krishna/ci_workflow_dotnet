using tester.Models;

namespace tester.Repositories;

public interface IMeterReadingRepository
{
    MeterReading Add(MeterReading reading);
    IEnumerable<MeterReading> GetAll();
    IEnumerable<MeterReading> GetByMeterId(string meterId);
    MeterReading? GetById(int id);
}
