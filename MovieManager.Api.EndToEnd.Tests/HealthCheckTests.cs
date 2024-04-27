using System.Net;

namespace MovieManager.Api.EndToEnd.Tests
{
    public class Tests
    {
        [TestFixture]
        public class HealthCheckTests
        {
            [Test]
            public async Task Call_HealthCheck_Endpoint()
            {
                var rootPath = "";
                var response = await HttpEndToEndClient.Create().GetAsync(rootPath);

                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("rest is: " + content);

                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(content, Is.EqualTo("Healthy"));
            }
        }
    }
}
