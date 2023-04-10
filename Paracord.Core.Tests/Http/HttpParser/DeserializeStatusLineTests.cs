using Paracord.Core.Http;

namespace Paracord.Core.Tests.Http.HttpParserTests
{
    public class DeserializeStatusLineTests
    {
        [Fact]
        public void DeserializeStatusLineThrowsArgumentNullExceptionWhenRequestIsNull()
        {
            // Arrange
            HttpRequest request = null!;

            // Act
            Action act = () => HttpParser.DeserializeStatusLine(request, new string[] { });

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void DeserializeStatusLineThrowsNotImplementedExceptionWhenHeaderDataIsEmpty()
        {
            // Arrange
            HttpRequest request = new HttpRequest();

            // Act
            Action act = () => HttpParser.DeserializeStatusLine(request, new string[] { });

            // Assert
            act.Should().Throw<NotImplementedException>();
        }

        [Fact]
        public void DeserializeStatusLineThrowsFormatExceptionWhenHeaderDataIsLessThan3Segments()
        {
            // Arrange
            HttpRequest request = new HttpRequest();
            string[] headerData =
            {
                "GET /"
            };

            // Act
            Action act = () => HttpParser.DeserializeStatusLine(request, headerData);

            // Assert
            act.Should().Throw<FormatException>();
        }

        [Fact]
        public void DeserializeStatusLineThrowsFormatExceptionWhenHeaderDataIsMoreThan3Segments()
        {
            // Arrange
            HttpRequest request = new HttpRequest();
            string[] headerData =
            {
                "GET / HTTP/1.1 ETag"
            };

            // Act
            Action act = () => HttpParser.DeserializeStatusLine(request, headerData);

            // Assert
            act.Should().Throw<FormatException>();
        }

        [Fact]
        public void DeserializeStatusLineThrowsNotImplementedExceptionWhenVerbIsInvalid()
        {
            // Arrange
            HttpRequest request = new HttpRequest();
            string[] headerData =
            {
                "RETRIEVE / HTTP/1.1"
            };

            // Act
            Action act = () => HttpParser.DeserializeStatusLine(request, headerData);

            // Assert
            act.Should().Throw<NotImplementedException>();
        }
    }
}