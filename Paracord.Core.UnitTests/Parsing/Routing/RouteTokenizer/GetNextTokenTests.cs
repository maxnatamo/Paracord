using System.Text;
using Paracord.Core.Parsing.Routing;

namespace Paracord.Core.UnitTests.Parsing.Routing.RouteTokenizerTests
{
    public class GetNextTokenTests
    {
        [Fact]
        public void GetNextTokenReturnsEndOfFileTokenGivenEmptyString()
        {
            // Arrange
            string route = string.Empty;

            // Act
            RouteToken token = new RouteTokenizer(route).GetNextToken();

            // Assert
            token.Start.Should().Be(0);
            token.End.Should().Be(0);
            token.Type.Should().Be(RouteTokenType.EOF);
            token.Value.Should().Be(string.Empty);
        }

        [Fact]
        public void GetNextTokenThrowsInvalidDataExceptionGivenNonAlnumByteSequence()
        {
            // Arrange
            string route = new ASCIIEncoding().GetString(new byte[] { 0x10 });

            // Act
            Action act = () => new RouteTokenizer(route).GetNextToken();

            // Assert
            act.Should().Throw<InvalidDataException>();
        }

        [Fact]
        public void GetNextTokenThrowsInvalidDataExceptionGivenNewline()
        {
            // Arrange
            string route = "\n";

            // Act
            Action act = () => new RouteTokenizer(route).GetNextToken();

            // Assert
            act.Should().Throw<InvalidDataException>();
        }

        [Fact]
        public void GetNextTokenReturnsEqualTokenGivenEqual()
        {
            // Arrange
            string route = "=";

            // Act
            RouteToken token = new RouteTokenizer(route).GetNextToken();

            // Assert
            token.Start.Should().Be(0);
            token.End.Should().Be(1);
            token.Type.Should().Be(RouteTokenType.EQUAL);
        }

        [Fact]
        public void GetNextTokenReturnsSlashTokenGivenForwardSlash()
        {
            // Arrange
            string route = "/";

            // Act
            RouteToken token = new RouteTokenizer(route).GetNextToken();

            // Assert
            token.Start.Should().Be(0);
            token.End.Should().Be(1);
            token.Type.Should().Be(RouteTokenType.SLASH);
        }

        [Fact]
        public void GetNextTokenReturnsBraceLeftTokenGivenBraceLeft()
        {
            // Arrange
            string route = "{";

            // Act
            RouteToken token = new RouteTokenizer(route).GetNextToken();

            // Assert
            token.Start.Should().Be(0);
            token.End.Should().Be(1);
            token.Type.Should().Be(RouteTokenType.BRACE_LEFT);
        }

        [Fact]
        public void GetNextTokenReturnsBraceRightTokenGivenBraceRight()
        {
            // Arrange
            string route = "}";

            // Act
            RouteToken token = new RouteTokenizer(route).GetNextToken();

            // Assert
            token.Start.Should().Be(0);
            token.End.Should().Be(1);
            token.Type.Should().Be(RouteTokenType.BRACE_RIGHT);
        }

        [Fact]
        public void GetNextTokenReturnsNameTokenGivenWord()
        {
            // Arrange
            string route = "Controller";

            // Act
            RouteToken token = new RouteTokenizer(route).GetNextToken();

            // Assert
            token.Start.Should().Be(0);
            token.End.Should().Be(10);
            token.Type.Should().Be(RouteTokenType.NAME);
            token.Value.Should().Be("Controller");
        }
    }
}