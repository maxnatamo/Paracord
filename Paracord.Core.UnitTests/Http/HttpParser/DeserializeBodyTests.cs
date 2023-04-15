using System.Text;

using Paracord.Core.Http;

namespace Paracord.Core.UnitTests.Http.HttpParserTests
{
    public class DeserializeBodyTests
    {
        [Fact]
        public void DeserializeBodyThrowsArgumentNullExceptionWhenRequestIsNull()
        {
            // Arrange
            HttpRequest request = null!;

            // Act
            Action act = () => HttpParser.DeserializeBody(request, new byte[] { });

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void DeserializeBodyThrowsArgumentNullExceptionWhenRequestBodyIsNull()
        {
            // Arrange
            HttpRequest request = new HttpRequest
            {
                Body = null!
            };

            // Act
            Action act = () => HttpParser.DeserializeBody(request, new byte[] { });

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void DeserializeBodyThrowsNotImplementedExceptionWhenBodyLengthIsDifferentFromContentLength()
        {
            // Arrange
            HttpRequest request = new HttpRequest();
            request.Headers.Add("content-length", "1");
            string body = @"Some value";

            // Act
            Action act = () => HttpParser.DeserializeBody(request, Encoding.ASCII.GetBytes(body));

            // Assert
            act.Should().Throw<NotImplementedException>();
        }

        [Fact]
        public void DeserializeBodyGivesCorrectBodyContentLengthGivenStringContent()
        {
            // Arrange
            string body = @"Some value";
            HttpRequest request = new HttpRequest();
            request.Headers.Add("content-length", body.Length.ToString());

            // Act
            HttpParser.DeserializeBody(request, Encoding.ASCII.GetBytes(body));

            // Assert
            request.Body.Seek(0, SeekOrigin.Begin);
            byte[] bodyContent = new byte[request.Body.Length];

            request.Body.Read(bodyContent).Should().Be(body.Length);
            request.Body.Length.Should().Be((long) body.Length);
            Encoding.ASCII.GetString(bodyContent).Should().Be(body);
        }
    }
}