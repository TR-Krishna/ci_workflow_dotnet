# tester

A small ASP.NET Core MVC-style Web API for practicing test automation and
CI/CD with GitHub Actions. Domain: recording meter readings and calculating
consumption - deliberately close to your real EcoSentinel/microservices work
so the patterns transfer directly.

## Architecture

```
src/tester/
  Controllers/   -> MeterReadingsController (thin, delegates to the service)
  Services/      -> IMeterReadingService / MeterReadingService (business logic)
  Repositories/  -> IMeterReadingRepository / InMemoryMeterReadingRepository (data access)
  Models/        -> MeterReading (domain model)
  Program.cs     -> DI wiring, minimal hosting

tests/tester.Tests/
  Services/      -> unit tests for MeterReadingService, mocking the repository
  Controllers/   -> unit tests for the controller, mocking the service
  Integration/   -> full in-process HTTP tests via WebApplicationFactory
```

This is a classic 3-layer MVC/API architecture: **Controller -> Service -> Repository**,
each depending only on the interface below it. That's what lets you unit-test
the Service by mocking `IMeterReadingRepository`, and unit-test the Controller
by mocking `IMeterReadingService`, without ever touching real infrastructure.

## Prerequisites

- .NET 8 SDK installed (`dotnet --version` should show `8.x`)
- Works fine on your existing Windows/PowerShell setup

## Running locally

```powershell
# from the repo root
dotnet restore
dotnet build
dotnet run --project src/tester
```

Then browse to `https://localhost:7080/swagger` to try the endpoints:
- `GET /api/meterreadings`
- `GET /api/meterreadings/{meterId}`
- `GET /api/meterreadings/{meterId}/average-consumption`
- `POST /api/meterreadings` with a JSON body like:
  ```json
  { "meterId": "MTR-001", "value": 100, "timestamp": "2026-07-01T00:00:00Z" }
  ```

## Running the tests

```powershell
dotnet test
```

You should see the worked-example tests pass, and you'll see the `// TODO (practice #N)`
comments marking exactly where to add your own.

## Practice exercises

Work through these roughly in order. Each one follows the same loop:
**write a failing test that proves the bug exists -> confirm it's red -> fix the
code -> confirm it's green.** That loop (red -> green -> refactor) is the core
skill, more important than any specific syntax.

| # | Location | What to do |
|---|---|---|
| 1 | `Services/MeterReadingServiceTests.cs` | Prove `RecordReading` currently accepts a negative value. Then fix `MeterReadingService` to reject it. |
| 2 | `Services/MeterReadingServiceTests.cs` | Prove `CalculateAverageConsumption` misbehaves (NaN/Infinity/exception) with zero readings. Decide the correct behavior and implement it. |
| 3 | `Services/MeterReadingServiceTests.cs` | Same as #2, but with exactly one reading. |
| 4 | `Services/MeterReadingServiceTests.cs` | Write a pure delegation test for `GetReadingsForMeter` using `Verify`. |
| 5 | `Controllers/MeterReadingsControllerTests.cs` | Assert `Post` returns `201 CreatedAtActionResult` on success, not just any 200. |
| 6 | `Controllers/MeterReadingsControllerTests.cs` | Test `GetAverageConsumption` returns `OkObjectResult` wrapping the mocked value. |
| 7 | `Integration/MeterReadingsApiIntegrationTests.cs` | End-to-end: post two readings, hit the average-consumption endpoint, check the math over real HTTP. |

**Bonus round** (once the above are green):
- Add a `[Trait("Category", "Unit")]` / `[Trait("Category", "Integration")]` to
  separate the two test types, then update the workflow to run unit tests on
  every push but integration tests only on `main`.
- Add a coverage threshold check step to the workflow (see the guide from
  earlier in this conversation) and intentionally drop it below 70% to watch
  the pipeline go red.
- Swap `InMemoryMeterReadingRepository` for a fake backed by EF Core's
  in-memory provider, and notice none of the Controller or Service tests need
  to change - that's the payoff of the layered architecture.

## CI/CD

`.github/workflows/dotnet-ci.yml` is preconfigured to:
1. Restore, build, and run all tests on every PR and push to `main`.
2. Publish a Test Results check on the PR (pass/fail per test, visible inline).
3. Collect code coverage and upload an HTML report as a downloadable artifact.

It's set to `runs-on: self-hosted` to match your existing runner at
`C:\actions-runner` - change it to `windows-latest` if you'd rather see it run
on GitHub's own infrastructure first with zero local setup.

To try it: push this repo to GitHub, open a PR that changes something in
`src/`, and watch the `dotnet-ci` workflow run under the **Actions** tab.
