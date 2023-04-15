using System.Text;

using Paracord.Core.Http;

namespace Paracord.Core.UnitTests.Http.HttpParserTests
{
    public class DeserializeResponseTests
    {
        [Fact]
        public void SerializeResponseReturnsGivenResponseWithoutHeadersOrBody()
        {
            // Arrange
            HttpResponse response = new HttpResponse();
            string expected = $"{HttpParser.CurrentHttpVersion.ToString()} 200\u000D\u000A\u000D\u000A";

            // Act
            byte[] received = HttpParser.SerializeResponse(response);

            // Assert
            received.Should().BeEquivalentTo(Encoding.ASCII.GetBytes(expected));
        }

        [Fact]
        public void SerializeResponseReturnsGivenResponseWithHeadersWithoutBody()
        {
            // Arrange
            HttpResponse response = new HttpResponse();
            response.Headers.Add("X-Sec-Header", "enabled");

            string expected = $"{HttpParser.CurrentHttpVersion.ToString()} 200\u000D\u000AX-Sec-Header: enabled\u000D\u000A\u000D\u000A";

            // Act
            byte[] received = HttpParser.SerializeResponse(response);

            // Assert
            received.Should().BeEquivalentTo(Encoding.ASCII.GetBytes(expected));
        }

        [Fact]
        public void SerializeResponseReturnsGivenResponseWithHeadersAndBody()
        {
            // Arrange
            HttpResponse response = new HttpResponse();
            response.Headers.Add("X-Sec-Header", "enabled");
            response.Body.Write(Encoding.ASCII.GetBytes("Text response."));

            string expected = $"{HttpParser.CurrentHttpVersion.ToString()} 200\u000D\u000AX-Sec-Header: enabled\u000D\u000A\u000D\u000AText response.";

            // Act
            byte[] received = HttpParser.SerializeResponse(response);

            // Assert
            received.Should().BeEquivalentTo(Encoding.ASCII.GetBytes(expected));
        }
    }
}