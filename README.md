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
