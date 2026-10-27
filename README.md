# dotnet-ping

.NET Tool for pinging URLs.

The tool supports using waiting between requests.
Can be configured using a JSON file which specifies URLs and their expected behaviors.

## Installation

Download the [latest version of .NET](https://dot.net).
Then install the [`dotnet-ping`](https://www.nuget.org/packages/dotnet-ping)
.NET Tool, using the command-line:

```
dotnet tool install -g dotnet-ping
```

## Usage

```
USAGE:
    dotnet-ping [urls] [OPTIONS]

EXAMPLES:
    dotnet-ping https://example.com
    dotnet-ping https://example.com other.com -s 1000
    dotnet-ping https://example.com other.com -s 1000 -s 2000 -t 5000
    dotnet-ping /about /contact -b https://example.com
    dotnet-ping -c ping.json

ARGUMENTS:
    [urls]    The URLs to ping. If not specified, the URLs are read from the JSON config file

OPTIONS:
    -h, --help        Prints help information
    -b, --base-url    Sets the base-URL to use for requests
    -c, --config      The path to the JSON config file
    -d, --debug       Use debug console messaging
    -e, --expect      Sets the expected status code of requests. Default: 200
    -X, --request     Sets the request method. Default: GET
    -m, --minimal     Use minimal console messaging
    -s, --sleep       Sets the sleep wait time between requests in milliseconds. Default: 500ms
    -t, --timeout     Sets the timeout for requests in milliseconds. Default: 5000ms. If two values are provided, a
                      random number between the two numbers will be generated for each request
```

### Examples

To ping a specific ULR:

```
dotnet ping https://example.com
```

To ping two ULRs and wait between 1 and 2 seconds between requests:

```
dotnet ping example.com example2.com/about -s 1000 -s 2000
```

To use a `ping.json` file, either navigate to the directory where the file is located and run `dotnet ping`,
or specify the full path to the file:

```
dotnet ping --config ping.json
```

### ping.json specification

This is the specification for the ping.json file. The only required field is `url`. Missing `https://` prefix will automatically be added.
All other fields have the same default values as the command line options.

```
{
    // Single URLs, with Single configurations
    "urls": [
        {
            "url": "test.com", // Required
            "method": "GET", // Default: GET
            "timeout": 15000, // Default: 5000ms
            "sleep": 100, // Default: 500ms
            "expect": [ 200, 201, 403 ] // Default: 200
        },
        {
            "url": "http://test2.com",
            "method": "DELETE",
            "timeout": 10000,
            "sleep": 200,
            "expect": [ 201, 202, 204 ]
        }
    ],
    // Groups of configurations, with single or multiple URLs
    "groups": [
        {
            "timeout": 20000,
            "sleep": 250,
            "expect": [ 301, 302 ],
            "urls": [ "test3.com/redirect", "test4.com/redirect" ]
        },
        {
            "timeout": 5000,
            "sleep": 550,
            "baseUrl": "https://test5.com/",
            "expect": [ 200, 201 ],
            "urls": [ "/", "/about", "/contact", "products" ]
        }
    ]
}

```

## Optional OpenTelemetry tracing

Tracing is off by default. With no Aient variables set, local development and installed
CLI usage create no telemetry provider or exporter and need no credentials or collector.
Setting only generic `OTEL_EXPORTER_OTLP_*` variables does not enable Aient tracing.

To opt in, set all three variables in the process environment:

| Variable | Value |
| --- | --- |
| `AIENT_ORG_ID` | Your Aient organization identifier |
| `AIENT_PUBLISHABLE_KEY` | Your Aient publishable key |
| `AIENT_OTLP_ENDPOINT` | Your verified full OTLP/HTTP trace URL, including `/v1/traces` where required |

There are no embedded organization, key, or endpoint defaults. Obtain the endpoint from
your collector or Aient configuration. The exporter uses HTTP/protobuf, sends the
organization and key in `x-aient-org-id` and `x-aient-publishable-key` headers, and records
outgoing HttpClient requests under service name `dotnet-ping`. Enabled tracing sends
request metadata to the configured collector. Use a collector you trust.

Use HTTPS for remote collectors. Plain HTTP is accepted only for loopback addresses
(`localhost`, `127.0.0.1`, or `::1`). Endpoints cannot contain user info, queries, or
fragments. Header values must be printable ASCII without whitespace, commas, or equals
signs. Partial, blank-required, or invalid configuration disables tracing with a short
stderr warning that never includes the supplied values. SDK initialization failures also
disable tracing without preventing CLI startup.

For a local collector, set the endpoint to `http://localhost:4318/v1/traces` and use
throwaway local organization/key values if the collector does not authenticate them.
The CLI does not load `.env` files. Supply values through your shell or development
runner, and use your deployment's secret store for deployed environments. Do not commit
real values to source, `ping.json`, launch settings, examples, or workflow files.
Set `OTEL_SDK_DISABLED=true` to disable tracing even when Aient variables are present.

The provider starts before command execution and is disposed on normal exit to flush
pending spans. Export HTTP requests have a one-second timeout. Collector delivery
failures do not change the command's result; a running collector is needed to verify
actual ingestion. Previously committed keys remain in Git history and should be rotated
or revoked with Aient before use.

### Focused verification

Install the .NET SDK selected by `global.json` and the .NET 9 runtime (or SDK), then run:

```sh
dotnet restore
dotnet test test/DotnetPing.Tests/DotnetPing.Tests.csproj -c Release --no-restore --filter "FullyQualifiedName~DotnetPing.Tests.Instrumentation"
```

These tests cover environment validation, safe warnings, disabled startup, real CLI help
startup, HTTP/protobuf export headers and payload, disposal flushing, exporter startup
failure, and instrumentation of an HTTP request to a loopback server. They use dummy
credentials and an in-memory exporter HTTP transport. No Aient account, external
collector, or Docker daemon is needed. The `Telemetry verification` CI job runs this
same focused path for both target frameworks.
