using System.Text;

using Paracord.Core.Http;
using Paracord.Core.Parsing.Http;

namespace Paracord.Core.Tests.Http.HttpParserTests
{
    public class DeserializeRequestTests
    {
        [Fact]
        public void DeserializeRequest_ThrowsFormatException_WhenNoContentDelimiterIsPresent()
        {
            // Arrange
            string request = @"\u000D\u000AGET / HTTP/1.1\u000D\u000AHost: localhost\u000D\u000Abody";

            // Act
            Action act = () => HttpParser.DeserializeRequest(new HttpRequest(), Encoding.ASCII.GetBytes(request));

            // Assert
            act.Should().Throw<FormatException>();
        }

        [Fact]
        public void DeserializeRequest_ThrowsFormatException_WhenTwoContentDelimiterIsPresent()
        {
            // Arrange
            string request = @"\u000D\u000AGET / HTTP/1.1\u000D\u000AHost: localhost\u000D\u000A\u000D\u000Abody\u000D\u000A\u000D\u000Abody 2";

            // Act
            Action act = () => HttpParser.DeserializeRequest(new HttpRequest(), Encoding.ASCII.GetBytes(request));

            // Assert
            act.Should().Throw<FormatException>();
        }
    }
}