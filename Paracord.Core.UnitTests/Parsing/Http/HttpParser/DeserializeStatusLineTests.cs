using Paracord.Core.Http;
using Paracord.Core.Parsing.Http;

namespace Paracord.Core.UnitTests.Http.HttpParserTests
{
    public class DeserializeStatusLineTests
    {
        [Fact]
        public void DeserializeStatusLine_ThrowsArgumentNullException_GivenNullRequest()
        {
            // Arrange
            HttpRequest request = null!;

            // Act
            Action act = () => HttpParser.DeserializeStatusLine(request, new string[] { });

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void DeserializeStatusLine_ThrowsNotImplementedException_EmptyHeaderData()
        {
            // Arrange
            HttpRequest request = new HttpRequest();

            // Act
            Action act = () => HttpParser.DeserializeStatusLine(request, new string[] { });

            // Assert
            act.Should().Throw<NotImplementedException>();
        }

        [Fact]
        public void DeserializeStatusLine_ThrowsFormatException_WhenHeaderDataIsLessThan3Segments()
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
        public void DeserializeStatusLine_ThrowsFormatException_WhenHeaderDataIsMoreThan3Segments()
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
        public void DeserializeStatusLine_ThrowsNotImplementedException_WhenVerbIsInvalid()
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