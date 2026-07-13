using System.Net;
using System.Net.Http.Json;
using tester.Controllers;
using tester.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace tester.Tests.Integration;

// This spins up the real app (real DI container, real routing, real in-memory
// repository) in-process and hits it over real HTTP. No mocks here - this is
// what catches "wiring" bugs (wrong route, wrong DI lifetime, serialization
// mismatches) that unit tests with mocks can't see.
public class MeterReadingsApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public MeterReadingsApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostThenGet_RoundTripsAReading()
    {
        // Arrange
        var request = new RecordReadingRequest("MTR-INTEGRATION", 123.4, DateTime.UtcNow);

        // Act
        var postResponse = await _client.PostAsJsonAsync("/api/meterreadings", request);
        var getResponse = await _client.GetAsync("/api/meterreadings/MTR-INTEGRATION");

        // Assert
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var readings = await getResponse.Content.ReadFromJsonAsync<List<MeterReading>>();
        Assert.Single(readings!);
        Assert.Equal("MTR-INTEGRATION", readings![0].MeterId);
    }

    // TODO (practice #7): GetAverageConsumption_EndToEnd
    // Post two readings for the same meterId via _client.PostAsJsonAsync, then
    // GET /api/meterreadings/{meterId}/average-consumption and assert the math
    // matches what you'd expect by hand.
}
