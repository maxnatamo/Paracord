using System.Text;

using Paracord.Core.Http;

namespace Paracord.Core.UnitTests.Http.HttpParserTests
{
    public class DeserializeRequestTests
    {
        [Fact]
        public void DeserializeRequestThrowsFormatExceptionWhenNoContentDelimiterIsPresent()
        {
            // Arrange
            string request = @"\u000D\u000AGET / HTTP/1.1\u000D\u000AHost: localhost\u000D\u000Abody";

            // Act
            Action act = () => HttpParser.DeserializeRequest(new HttpRequest(), Encoding.ASCII.GetBytes(request));

            // Assert
            act.Should().Throw<FormatException>();
        }

        [Fact]
        public void DeserializeRequestThrowsFormatExceptionWhenTwoContentDelimiterIsPresent()
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