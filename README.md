# dotnet-ping

.NET Tool for pinging URLs.

The tool supports using backoff between requests.
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
    dotnet ping [urls] [OPTIONS]

EXAMPLES:
    dotnet ping
    dotnet ping https://dot.net
    dotnet ping https://dot.net https://www.nuget.org -b 1000 2000
    dotnet ping -c ping.json

ARGUMENTS:
    [urls]              The URLs to ping. If not specified, the URLs are read from the ping.json file.

OPTIONS:
    -h, --help          Prints help information.
    -b, --backoff       Sets the backoff time in milliseconds. Default: 1000ms.
    -m, --backoff-max   Sets the max backoff time in milliseconds. Uses the backoff time as start value and this value as end value for a random number.
    -c, --config        The path to the ping.json file. Default: /ping.json.
```

### Examples

To ping a specific ULR:

```
dotnet ping https://dot.net
```

To ping two ULRs and wait between 1 and 2 seconds between requests:

```
dotnet ping https://dot.net --backoff 1000 2000
```

To use a `ping.json` file, either navigate to the directory where the file is located and run `dotnet ping`,
or specify the full path to the file:

```
dotnet ping --config ping.json
```
