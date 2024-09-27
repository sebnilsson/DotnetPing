namespace DotnetPing.Http;

public static class HttpMethodResolver
{
    public static HttpMethod Get(string? method)
    {
        if (string.IsNullOrWhiteSpace(method))
        {
            return HttpMethod.Get;
        }

        switch (method.ToLowerInvariant())
        {
            case "post":
                return HttpMethod.Post;
            case "put":
                return HttpMethod.Put;
            case "delete":
                return HttpMethod.Delete;
            case "patch":
                return HttpMethod.Patch;
            case "head":
                return HttpMethod.Head;
            case "options":
                return HttpMethod.Options;
        }

        return HttpMethod.Get;
    }
}
