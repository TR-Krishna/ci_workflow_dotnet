namespace tester.Models;

public class MeterReading
{
    public int Id { get; set; }
    public string MeterId { get; set; } = string.Empty;
    public double Value { get; set; }
    public DateTime Timestamp { get; set; }
}
