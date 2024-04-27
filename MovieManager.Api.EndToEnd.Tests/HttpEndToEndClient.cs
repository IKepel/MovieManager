using Polly;
namespace MovieManager.Api.EndToEnd.Tests
{
    public class HttpEndToEndClient
    {
        private readonly HttpClient _httpClient;

        private readonly int _maxRetryAttempts;

        public HttpEndToEndClient(int maxRetryAttempts)
        {
            _maxRetryAttempts = maxRetryAttempts;
            var baseUrl = "https://localhost:7205";

            Console.WriteLine($"baseUrl: {baseUrl}");

            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(baseUrl)
            };

            _httpClient.DefaultRequestHeaders.Add("x-correlation-id", Guid.NewGuid().ToString());
        }

        public static HttpEndToEndClient Create(int maxRetryAttempts = 5)
        {
            return new HttpEndToEndClient(maxRetryAttempts);
        }

        public async Task<HttpResponseMessage> GetAsync(string path)
        {
            Random jitterer = new();

            TimeSpan PauseBetweenFailures(int retryAttempt)
            {
                return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                       + TimeSpan.FromMilliseconds(jitterer.Next(0, 1000));
            }

            var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(_maxRetryAttempts, PauseBetweenFailures);

            //var retryPolicy = HttpPolicyExtensions
            //    .HandleTransientHttpError()
            //    .WaitAndRetryAsync(_maxRetryAttempts, PauseBetweenFailures);

            Console.WriteLine($"Calling endpoint: {_httpClient.BaseAddress}{path}");

            var response = await retryPolicy.ExecuteAsync(() => _httpClient.GetAsync(path));
            return response;
        }
    }
}