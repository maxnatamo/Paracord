using Moq;
using Paracord.Core.Http;
using Paracord.Core.Listener;
using Paracord.Core.Middleware;
using Paracord.Shared.Models.Http;
using Paracord.Shared.Models.Listener;

namespace Paracord.Core.IntegrationTests.Middleware.HttpsRedirectMiddlewareTests
{
    public class TestHttpContext : HttpContext
    {
        public TestHttpContext(HttpListener listener, ListenerPrefix listenerPrefix) : base(listener, listenerPrefix)
        {

        }
    }

    public class HttpsRedirectMiddlewareTests
    {
        [Fact]
        public void HttpsRedirectMiddlewareContinuesGivenSingleInsecurePrefixAndInsecureRequest()
        {
            // Arrange
            HttpsRedirectMiddleware middleware = new HttpsRedirectMiddleware();
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8080");

            HttpContext context = new TestHttpContext(listener, listener.Prefixes.First());

            Mock<HttpRequest> request = new Mock<HttpRequest>();
            request.Object.Context = context;

            Mock<HttpResponse> response = new Mock<HttpResponse>();
            response.Object.Context = context;

            // Act
            middleware.AfterRequestReceived(listener, request.Object, response.Object);

            // Assert
            response.Object.StatusCode.Should().Be(HttpStatusCode.Ok);
        }

        [Fact]
        public void HttpsRedirectMiddlewareContinuesGivenSingleSecurePrefixAndSecureRequest()
        {
            // Arrange
            HttpsRedirectMiddleware middleware = new HttpsRedirectMiddleware();
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("https://localhost:8080");

            HttpContext context = new TestHttpContext(listener, listener.Prefixes.First());

            Mock<HttpRequest> request = new Mock<HttpRequest>();
            request.Object.Context = context;

            Mock<HttpResponse> response = new Mock<HttpResponse>();
            response.Object.Context = context;

            // Act
            middleware.AfterRequestReceived(listener, request.Object, response.Object);

            // Assert
            response.Object.StatusCode.Should().Be(HttpStatusCode.Ok);
        }

        [Fact]
        public void HttpsRedirectMiddlewareRedirectsGivenSingleSecurePrefixAndInsecureRequest()
        {
            // Arrange
            HttpsRedirectMiddleware middleware = new HttpsRedirectMiddleware();
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("https://localhost:8080");

            Mock<TestHttpContext> context = new Mock<TestHttpContext>(listener, ListenerPrefix.Parse("http://localhost:8080"));

            Mock<HttpRequest> request = new Mock<HttpRequest>();
            request.Object.Context = context.Object;

            Mock<HttpResponse> response = new Mock<HttpResponse>();
            response.Object.Context = context.Object;
            response.Setup(v => v.Send()).Callback(() => { });

            // Act
            middleware.AfterRequestReceived(listener, request.Object, response.Object);

            // Assert
            response.Object.StatusCode.Should().Be(HttpStatusCode.MovedPermanently);
            response.Object.Headers[HttpHeaders.Location].Should().Be("https://localhost:8080");
        }

        [Fact]
        public void HttpsRedirectMiddlewareContinuesGivenUpgradeInsecureRequestsHeaderOfZero()
        {
            // Arrange
            HttpsRedirectMiddleware middleware = new HttpsRedirectMiddleware();
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("https://localhost:8080");

            Mock<TestHttpContext> context = new Mock<TestHttpContext>(listener, ListenerPrefix.Parse("http://localhost:8080"));

            Mock<HttpRequest> request = new Mock<HttpRequest>();
            request.Object.Context = context.Object;
            request.Object.Headers[HttpHeaders.UpgradeInsecureRequests] = "0";

            Mock<HttpResponse> response = new Mock<HttpResponse>();
            response.Object.Context = context.Object;
            response.Setup(v => v.Send()).Callback(() => { });

            // Act
            middleware.AfterRequestReceived(listener, request.Object, response.Object);

            // Assert
            response.Object.StatusCode.Should().Be(HttpStatusCode.Ok);
        }

        [Fact]
        public void HttpsRedirectMiddlewareRedirectsGivenUpgradeInsecureRequestsHeaderOfOne()
        {
            // Arrange
            HttpsRedirectMiddleware middleware = new HttpsRedirectMiddleware();
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("https://localhost:8080");

            Mock<TestHttpContext> context = new Mock<TestHttpContext>(listener, ListenerPrefix.Parse("http://localhost:8080"));

            Mock<HttpRequest> request = new Mock<HttpRequest>();
            request.Object.Context = context.Object;
            request.Object.Headers[HttpHeaders.UpgradeInsecureRequests] = "1";

            Mock<HttpResponse> response = new Mock<HttpResponse>();
            response.Object.Context = context.Object;
            response.Setup(v => v.Send()).Callback(() => { });

            // Act
            middleware.AfterRequestReceived(listener, request.Object, response.Object);

            // Assert
            response.Object.StatusCode.Should().Be(HttpStatusCode.MovedPermanently);
            response.Object.Headers[HttpHeaders.Location].Should().Be("https://localhost:8080");
        }
    }
}