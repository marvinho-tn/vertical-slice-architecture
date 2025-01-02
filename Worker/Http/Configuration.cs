using Refit;

namespace Worker.Http;

public class CustomExceptionHandler : DelegatingHandler
{
    public CustomExceptionHandler(HttpMessageHandler innerHandler)
    {
        InnerHandler = innerHandler;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            return response;
        }

        response.EnsureSuccessStatusCode();
        
        return response;
    }
}

public static class HttpExtensions
{
    public static T CreateHttpService<T>(string baseUri)
    {
        var httpClientHandler = new HttpClientHandler();
        var customExceptionHandler = new CustomExceptionHandler(httpClientHandler);
        var httpClient = new HttpClient(customExceptionHandler)
        {
            BaseAddress = new Uri(baseUri)
        };

        return RestService.For<T>(httpClient);
    }
}