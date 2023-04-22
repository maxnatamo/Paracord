using Moq;
using Paracord.Core.Http;
using Paracord.Core.Listener;
using Paracord.Core.Middleware;
using Paracord.Shared.Models.Http;
using Paracord.Shared.Models.Listener;

namespace Paracord.Core.IntegrationTests.Middleware.ETagMiddlewareTests
{
    public class ETagMiddlewareTests
    {
        [Fact]
        public void ETagMiddleware_Skips_GivenNonSuccessfulErrorCode()
        {
            // Arrange
            ETagMiddleware middleware = new ETagMiddleware();
            HttpListener listener = new HttpListener();

            Mock<HttpRequest> request = new Mock<HttpRequest>();
            Mock<HttpResponse> response = new Mock<HttpResponse>();
            response.Object.StatusCode = HttpStatusCode.Unauthorized;

            // Act
            middleware.BeforeResponseSent(listener, request.Object, response.Object);

            // Assert
            response.Object.Headers[HttpHeaders.ETag].Should().BeNull();
        }

        [Fact]
        public void ETagMiddleware_Skips_GivenAlreadyExistingETagHeader()
        {
            // Arrange
            ETagMiddleware middleware = new ETagMiddleware();
            HttpListener listener = new HttpListener();

            Mock<HttpRequest> request = new Mock<HttpRequest>();
            Mock<HttpResponse> response = new Mock<HttpResponse>();
            response.Object.Headers[HttpHeaders.ETag] = "0xDEADBEEF";

            // Act
            middleware.BeforeResponseSent(listener, request.Object, response.Object);

            // Assert
            response.Object.Headers[HttpHeaders.ETag].Should().Be("0xDEADBEEF");
        }

        [Fact]
        public void ETagMiddleware_Skips_GivenBodyLengthOverLimit()
        {
            // Arrange
            ETagMiddleware middleware = new ETagMiddleware();
            HttpListener listener = new HttpListener();

            Mock<HttpRequest> request = new Mock<HttpRequest>();
            Mock<HttpResponse> response = new Mock<HttpResponse>();
            response.Object.Write(string.Join("", Enumerable.Repeat("A", 40 * 1024)));

            // Act
            middleware.BeforeResponseSent(listener, request.Object, response.Object);

            // Assert
            response.Object.Headers[HttpHeaders.ETag].Should().BeNull();
        }

        [Fact]
        public void ETagMiddleware_ReturnsETag_GivenTextResponse()
        {
            // Arrange
            ETagMiddleware middleware = new ETagMiddleware();
            HttpListener listener = new HttpListener();

            HttpResponse response = new HttpResponse();
            response.StatusCode = HttpStatusCode.Ok;
            response.Write("PEBCAK");

            // Act
            middleware.BeforeResponseSent(listener, new HttpRequest(), response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Ok);
            response.Headers[HttpHeaders.ETag].Should().Be("\"ksm17orqgGGqa3+l55bZMpfEkbhN6GbWHK41YF08DgE=\"");
        }

        [Fact]
        public void ETagMiddleware_ReturnsETag_GivenTextResponseAndMatchingIfMatchHeader()
        {
            // Arrange
            ETagMiddleware middleware = new ETagMiddleware();
            HttpListener listener = new HttpListener();

            HttpResponse response = new HttpResponse();
            response.Headers[HttpHeaders.IfMatch] = "\"ksm17orqgGGqa3+l55bZMpfEkbhN6GbWHK41YF08DgE=\"";
            response.Write("PEBCAK");

            // Act
            middleware.BeforeResponseSent(listener, new HttpRequest(), response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Ok);
            response.Headers[HttpHeaders.ETag].Should().Be("\"ksm17orqgGGqa3+l55bZMpfEkbhN6GbWHK41YF08DgE=\"");
        }

        [Fact]
        public void ETagMiddleware_ReturnsETag_GivenCatchAllIfMatchHeader()
        {
            // Arrange
            ETagMiddleware middleware = new ETagMiddleware();
            HttpListener listener = new HttpListener();

            HttpResponse response = new HttpResponse();
            response.Headers[HttpHeaders.IfMatch] = "*";
            response.Write("You're lucky it compiled");

            // Act
            middleware.BeforeResponseSent(listener, new HttpRequest(), response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Ok);
            response.Headers[HttpHeaders.ETag].Should().Be("\"CUGRsNzUr+RK0Omi/iTpTJlJ01nbq+fN8k9P8wNB5I4=\"");
        }

        [Fact]
        public void ETagMiddleware_ReturnsPreconditionFailed_GivenNonMatchingIfMatchHeader()
        {
            // Arrange
            ETagMiddleware middleware = new ETagMiddleware();
            HttpListener listener = new HttpListener();

            HttpResponse response = new HttpResponse();
            response.Write("Works on my machine.");

            HttpRequest request = new HttpRequest();
            request.Headers[HttpHeaders.IfMatch] = "\"NON_MATCHING_HASH\"";

            // Act
            middleware.BeforeResponseSent(listener, request, response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
            response.Headers[HttpHeaders.ETag].Should().Be("\"GnlJmzCjoNoet2frl3krEp/aDjwVtRXLNgdzs8d3tOU=\"");
        }

        [Fact]
        public void ETagMiddleware_ReturnsETag_GivenNonMatchingIfNoneMatchHeader()
        {
            // Arrange
            ETagMiddleware middleware = new ETagMiddleware();
            HttpListener listener = new HttpListener();

            HttpResponse response = new HttpResponse();
            response.Write("Level 8 error occured");

            HttpRequest request = new HttpRequest();
            request.Headers[HttpHeaders.IfNoneMatch] = "\"NOT_CORRECT_HASH\"";

            // Act
            middleware.BeforeResponseSent(listener, request, response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Ok);
            response.Headers[HttpHeaders.ETag].Should().Be("\"5F0UiT5ZfjHpJRc8nPrSvKCPb47TPxpq1/qn67TvMAw=\"");
        }

        [Fact]
        public void ETagMiddleware_ReturnsPreconditionFailed_GivenMatchingIfNoneMatchHeader()
        {
            // Arrange
            ETagMiddleware middleware = new ETagMiddleware();
            HttpListener listener = new HttpListener();

            HttpResponse response = new HttpResponse();
            response.Write("Still in Indev");

            HttpRequest request = new HttpRequest();
            request.Headers[HttpHeaders.IfNoneMatch] = "\"Bvm+kylv3eVqvxKsqNqKbf7O0PjgxLxGQnTWONxLDPU=\"";

            // Act
            middleware.BeforeResponseSent(listener, request, response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
            response.Headers[HttpHeaders.ETag].Should().Be("\"Bvm+kylv3eVqvxKsqNqKbf7O0PjgxLxGQnTWONxLDPU=\"");
        }
    }
}