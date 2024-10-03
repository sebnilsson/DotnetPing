
namespace DotnetPing.Ping;

public class PingResults
{
    public PingResults(PingResult[] results)
    {
        All = results;
        Successes = results.Where(x => x.Result == PingResultType.Success).ToArray();
        Failures = results.Where(x => x.Result == PingResultType.Failure).ToArray();
        Timeouts = results.Where(x => x.Result == PingResultType.Timeout).ToArray();
        ResultType = GetResultType();
    }

    public PingResult[] All { get; }

    public PingResult[] Failures { get; private set; }

    public PingResult[] Successes { get; }

    public PingResult[] Timeouts { get; }

    public PingResultType ResultType { get; }

    private PingResultType GetResultType()
    {
        if (All.Length == 0)
        {
            return PingResultType.None;
        }

        if (Failures.Length > 0)
        {
            return PingResultType.Failure;
        }

        if (Timeouts.Length > 0)
        {
            return PingResultType.Timeout;
        }

        return PingResultType.Success;
    }

    public static PingResults Empty => new([]);
}
