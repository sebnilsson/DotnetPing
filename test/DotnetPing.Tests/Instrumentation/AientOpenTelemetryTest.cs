using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using DotnetPing.Instrumentation;

namespace DotnetPing.Tests.Instrumentation;

[Collection("Telemetry")]
public class AientOpenTelemetryTest
{
    [Fact]
    public void Start_Unconfigured_DoesNotCreateExporter()
    {
        // Arrange
        var warnings = new List<string>();
        var exporterCreated = false;

        // Act
        using var provider = AientOpenTelemetry.Start(_ => null, warnings.Add, () =>
        {
            exporterCreated = true;
            throw new InvalidOperationException();
        });

        // Assert
        Assert.Null(provider);
        Assert.False(exporterCreated);
        Assert.Empty(warnings);
    }

    [Fact]
    public void Start_Dispose_FlushesHttpProtobufWithConfiguredHeadersAndService()
    {
        // Arrange
        var environment = AientTelemetrySettingsTest.ValidEnvironment();
        var handler = new CaptureHandler();
        var warnings = new List<string>();
        using var source = new ActivitySource("DotnetPing");

        // Act
        using (var provider = AientOpenTelemetry.Start(
            name => environment.GetValueOrDefault(name), warnings.Add, () => new HttpClient(handler)))
        {
            Assert.NotNull(provider);
            using var activity = source.StartActivity("verification");
            Assert.NotNull(activity);
        }

        // Assert
        Assert.Empty(warnings);
        var request = Assert.Single(handler.Requests);
        Assert.Equal(new Uri(environment["AIENT_OTLP_ENDPOINT"]), request.Endpoint);
        Assert.Equal("application/x-protobuf", request.ContentType);
        Assert.Equal("test-org", request.OrganizationId);
        Assert.Equal("test-key", request.PublishableKey);
        var body = Encoding.UTF8.GetString(request.Body);
        Assert.Contains("dotnet-ping", body);
        Assert.Contains("verification", body);
        Assert.DoesNotContain("test-key", body);
    }

    [Fact]
    public async Task Start_RealHttpClientRequest_IsInstrumented()
    {
        // Arrange
        var environment = AientTelemetrySettingsTest.ValidEnvironment();
        var handler = new CaptureHandler();
        using var provider = AientOpenTelemetry.Start(
            name => environment.GetValueOrDefault(name), exporterHttpClientFactory: () => new HttpClient(handler));
        Assert.NotNull(provider);
        using var server = new TcpListener(IPAddress.Loopback, 0);
        server.Start();
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var serverTask = RespondAsync(server, timeout.Token);
        using var client = new HttpClient(new SocketsHttpHandler { UseProxy = false });
        var port = ((IPEndPoint)server.LocalEndpoint).Port;

        // Act
        using var response = await client.GetAsync($"http://127.0.0.1:{port}/ping", timeout.Token);
        await serverTask;
        var flushed = provider.ForceFlush(5_000);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(flushed);
        var body = string.Concat(handler.Requests.Select(request => Encoding.UTF8.GetString(request.Body)));
        Assert.Contains("System.Net.Http", body);
        Assert.Contains("/ping", body);
    }

    [Fact]
    public void Start_ExporterInitializationFails_WarnsWithoutLeakingException()
    {
        // Arrange
        var environment = AientTelemetrySettingsTest.ValidEnvironment();
        var warnings = new List<string>();

        // Act
        using var provider = AientOpenTelemetry.Start(
            name => environment.GetValueOrDefault(name), warnings.Add,
            () => throw new InvalidOperationException("test-key"));

        // Assert
        Assert.Null(provider);
        var warning = Assert.Single(warnings);
        Assert.Contains("initialization failed", warning);
        Assert.DoesNotContain("test-key", warning);
    }

    private static async Task RespondAsync(TcpListener listener, CancellationToken cancellationToken)
    {
        using var connection = await listener.AcceptTcpClientAsync(cancellationToken);
        await using var stream = connection.GetStream();
        var buffer = new byte[4_096];
        var headers = new StringBuilder();
        while (!headers.ToString().Contains("\r\n\r\n", StringComparison.Ordinal))
        {
            var count = await stream.ReadAsync(buffer, cancellationToken);
            if (count == 0)
            {
                throw new IOException("Client closed before sending HTTP headers.");
            }

            headers.Append(Encoding.ASCII.GetString(buffer, 0, count));
        }

        await stream.WriteAsync("HTTP/1.1 200 OK\r\nContent-Length: 2\r\nConnection: close\r\n\r\nOK"u8.ToArray(), cancellationToken);
    }

    private sealed record ExportedRequest(Uri? Endpoint, string? ContentType, string OrganizationId, string PublishableKey, byte[] Body);

    private sealed class CaptureHandler : HttpMessageHandler
    {
        public ConcurrentQueue<ExportedRequest> Requests { get; } = new();

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var content = request.Content!;
            Requests.Enqueue(new ExportedRequest(
                request.RequestUri,
                content.Headers.ContentType?.MediaType,
                request.Headers.GetValues("x-aient-org-id").Single(),
                request.Headers.GetValues("x-aient-publishable-key").Single(),
                await content.ReadAsByteArrayAsync(cancellationToken)));
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent([]) };
        }
    }
}
