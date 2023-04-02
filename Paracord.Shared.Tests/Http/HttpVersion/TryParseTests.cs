using Paracord.Shared.Http;

namespace Paracord.Shared.Tests.Http.HttpVersionTests
{
    public class TryParseTests
    {
        [Fact]
        public void TryParseReturnsFalseGivenEmptyString()
        {
            // Arrange
            string value = string.Empty;

            // Act
            bool parsed = HttpVersion.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParseReturnsFalseGivenWhitespace()
        {
            // Arrange
            string value = "  \t\t\n";

            // Act
            bool parsed = HttpVersion.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParseReturnsTrueGivenHTTP_1_1()
        {
            // Arrange
            string value = "HTTP/1.1";

            // Act
            bool parsed = HttpVersion.TryParse(value, out var version);

            // Assert
            parsed.Should().BeTrue();
            version.Should().NotBeNull();
            version?.Major.Should().Be(1);
            version?.Minor.Should().Be(1);
        }

        [Fact]
        public void TryParseReturnsFalseGivenHTTP_1_11()
        {
            // Arrange
            string value = "HTTP/1.11";

            // Act
            bool parsed = HttpVersion.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParseReturnsFalseGivenHTTP_1_A()
        {
            // Arrange
            string value = "HTTP/1.A";

            // Act
            bool parsed = HttpVersion.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParseReturnsFalseGivenHTTP_1_()
        {
            // Arrange
            string value = "HTTP/1.";

            // Act
            bool parsed = HttpVersion.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }
    }
}