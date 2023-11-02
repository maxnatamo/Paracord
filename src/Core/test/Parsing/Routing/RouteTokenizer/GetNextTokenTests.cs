using System.Text;
using Paracord.Core.Parsing.Routing;

namespace Paracord.Core.Tests.Parsing.Routing.RouteTokenizerTests
{
    public class GetNextTokenTests
    {
        [Fact]
        public void GetNextToken_ReturnsEndOfFileToken_GivenEmptyString()
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
        public void GetNextTokenThrowsInvalidDataException_GivenNonAlnumByteSequence()
        {
            // Arrange
            string route = new ASCIIEncoding().GetString(new byte[] { 0x10 });

            // Act
            Action act = () => new RouteTokenizer(route).GetNextToken();

            // Assert
            act.Should().Throw<InvalidDataException>();
        }

        [Fact]
        public void GetNextTokenThrowsInvalidDataException_GivenNewline()
        {
            // Arrange
            string route = "\n";

            // Act
            Action act = () => new RouteTokenizer(route).GetNextToken();

            // Assert
            act.Should().Throw<InvalidDataException>();
        }

        [Fact]
        public void GetNextToken_ReturnsEqualToken_GivenEqual()
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
        public void GetNextToken_ReturnsColonToken_GivenColon()
        {
            // Arrange
            string route = ":";

            // Act
            RouteToken token = new RouteTokenizer(route).GetNextToken();

            // Assert
            token.Start.Should().Be(0);
            token.End.Should().Be(1);
            token.Type.Should().Be(RouteTokenType.COLON);
        }

        [Fact]
        public void GetNextToken_ReturnsSlashToken_GivenForwardSlash()
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
        public void GetNextToken_ReturnsBraceLeftToken_GivenBraceLeft()
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
        public void GetNextToken_ReturnsBraceRightToken_GivenBraceRight()
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
        public void GetNextToken_ReturnsNameToken_GivenWord()
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

        [Fact]
        public void GetNextToken_ReturnsUnknownToken_GivenUnsupportedToken()
        {
            // Arrange
            string route = "%";

            // Act
            RouteToken token = new RouteTokenizer(route).GetNextToken();

            // Assert
            token.Start.Should().Be(0);
            token.End.Should().Be(1);
            token.Type.Should().Be(RouteTokenType.UNKNOWN);
            token.Value.Should().Be("%");
        }
    }
}