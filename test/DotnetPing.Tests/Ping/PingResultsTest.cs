using DotnetPing.Http;
using DotnetPing.Ping;

namespace DotnetPing.Tests.Ping;

public class PingResultsTest
{
    private static PingResult CreateResult(PingResultType type)
    {
        var url = new UrlConfig("https://example.com", "GET", new Config());
        var http = new HttpResult("https://example.com", type == PingResultType.Success ? 200u : 0u, IsTimeout: type == PingResultType.Timeout);

        // The PingResult constructor decides PingResultType based on isSuccess + IsTimeout, so wire arguments accordingly.
        var isSuccess = type == PingResultType.Success;
        var result = new PingResult(isSuccess, TimeSpan.Zero, http, url);

        Assert.Equal(type, result.Result);
        return result;
    }

    [Fact]
    public void Empty_ReportsNone()
    {
        // Arrange

        // Act
        var results = PingResults.Empty;

        // Assert
        Assert.Empty(results.All);
        Assert.Empty(results.Successes);
        Assert.Empty(results.Failures);
        Assert.Empty(results.Timeouts);
        Assert.Equal(PingResultType.None, results.ResultType);
    }

    [Fact]
    public void AllSuccess_ReportsSuccess()
    {
        // Arrange

        // Act
        var results = new PingResults([CreateResult(PingResultType.Success), CreateResult(PingResultType.Success)]);

        // Assert
        Assert.Equal(2, results.Successes.Length);
        Assert.Empty(results.Failures);
        Assert.Empty(results.Timeouts);
        Assert.Equal(PingResultType.Success, results.ResultType);
    }

    [Fact]
    public void AnyFailure_ReportsFailure_EvenWithSuccessesAndTimeouts()
    {
        // Arrange

        // Act
        var results = new PingResults(
        [
            CreateResult(PingResultType.Success),
            CreateResult(PingResultType.Failure),
            CreateResult(PingResultType.Timeout),
        ]);

        // Assert
        Assert.Single(results.Successes);
        Assert.Single(results.Failures);
        Assert.Single(results.Timeouts);
        Assert.Equal(PingResultType.Failure, results.ResultType);
    }

    [Fact]
    public void TimeoutWithoutFailure_ReportsTimeout()
    {
        // Arrange

        // Act
        var results = new PingResults(
        [
            CreateResult(PingResultType.Success),
            CreateResult(PingResultType.Timeout),
        ]);

        // Assert
        Assert.Equal(PingResultType.Timeout, results.ResultType);
    }
}
