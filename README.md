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
TODO: Copy paste from `dotnet ping --help`
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
