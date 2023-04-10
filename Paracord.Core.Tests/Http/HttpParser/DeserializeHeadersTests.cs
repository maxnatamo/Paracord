using Paracord.Core.Http;

namespace Paracord.Core.Tests.Http.HttpParserTests
{
    public class DeserializeHeadersTests
    {
        [Fact]
        public void DeserializeHeadersThrowsArgumentNullExceptionWhenRequestIsNull()
        {
            // Arrange
            HttpRequest request = null!;

            // Act
            Action act = () => HttpParser.DeserializeBody(request, new byte[] { });

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void DeserializeHeadersThrowsArgumentNullExceptionWhenRequestHeadersIsNull()
        {
            // Arrange
            HttpRequest request = new HttpRequest
            {
                Headers = null!
            };

            // Act
            Action act = () => HttpParser.DeserializeHeaders(request, new string[] { });

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void DeserializeHeadersReturnsEmptyHeadersWhenListIsEmpty()
        {
            // Arrange
            HttpRequest request = new HttpRequest();
            string[] headerData =
            {
                "Status-Line"
            };

            // Act
            HttpParser.DeserializeHeaders(request, headerData);

            // Assert
            request.Headers.HasKeys().Should().BeFalse();
        }

        [Fact]
        public void DeserializeHeadersReturnsLoweredHeaderNames()
        {
            // Arrange
            HttpRequest request = new HttpRequest();
            string[] headerData =
            {
                "Status-Line",
                "X-Sec-Header: enabled"
            };

            // Act
            HttpParser.DeserializeHeaders(request, headerData);

            // Assert
            request.Headers.HasKeys().Should().BeTrue();
            request.Headers.Count.Should().Be(1);
            request.Headers["x-sec-header"].Should().NotBeNull();
            request.Headers["X-Sec-Header"].Should().NotBeNull();
        }

        [Fact]
        public void DeserializeHeadersThrowsFormatExceptionIfNoValueIsSpecified()
        {
            // Arrange
            HttpRequest request = new HttpRequest();
            string[] headerData =
            {
                "Status-Line",
                "X-Sec-Header"
            };

            // Act
            Action act = () => HttpParser.DeserializeHeaders(request, headerData);

            // Assert
            act.Should().Throw<FormatException>();
        }

        [Fact]
        public void DeserializeHeadersReturnsRequestGivenHeaderWithWhitespace()
        {
            // Arrange
            HttpRequest request = new HttpRequest();
            string[] headerData =
            {
                "Status-Line",
                "X-Sec-Header:   enabled  "
            };

            // Act
            HttpParser.DeserializeHeaders(request, headerData);

            // Assert
            request.Headers.HasKeys().Should().BeTrue();
            request.Headers["x-sec-header"].Should().Be("enabled");
        }

        [Fact]
        public void DeserializeHeadersReturnsRequestGivenHeaderWithMultipleColons()
        {
            // Arrange
            HttpRequest request = new HttpRequest();
            string[] headerData =
            {
                "Status-Line",
                "X-Sec-Header: enabled: disabled"
            };

            // Act
            HttpParser.DeserializeHeaders(request, headerData);

            // Assert
            request.Headers.HasKeys().Should().BeTrue();
            request.Headers["x-sec-header"].Should().Be("enabled: disabled");
        }
    }
}