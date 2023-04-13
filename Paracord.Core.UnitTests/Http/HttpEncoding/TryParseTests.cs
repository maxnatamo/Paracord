using Paracord.Core.Http;

namespace Paracord.Core.Tests.Http.HttpEncodingTests
{
    public class TryParseTests
    {
        [Fact]
        public void TryParseReturnsFalseGivenEmptyString()
        {
            // Arrange
            string value = string.Empty;

            // Act
            bool parsed = HttpEncoding.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParseReturnsTrueGivenOnlyEncoding()
        {
            // Arrange
            string value = "gzip";

            // Act
            bool parsed = HttpEncoding.TryParse(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result.Should().NotBeNull();
            result?.Encoding.Should().Be("gzip");
            result?.Weight.Should().Be(1.0f);
        }

        [Fact]
        public void TryParseReturnsTrueGivenEncodingAndWeight()
        {
            // Arrange
            string value = "gzip;q=0.9";

            // Act
            bool parsed = HttpEncoding.TryParse(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result.Should().NotBeNull();
            result?.Encoding.Should().Be("gzip");
            result?.Weight.Should().Be(0.9f);
        }

        [Fact]
        public void TryParseReturnsFalseGivenOnlyWeight()
        {
            // Arrange
            string value = "q=0.9";

            // Act
            bool parsed = HttpEncoding.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParseReturnsFalseGivenOnlyWeightWithSemicolon()
        {
            // Arrange
            string value = ";q=0.9";

            // Act
            bool parsed = HttpEncoding.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParseReturnsFalseGivenOnlySemicolon()
        {
            // Arrange
            string value = ";";

            // Act
            bool parsed = HttpEncoding.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParseReturnsFalseGivenEncodingAndSemicolon()
        {
            // Arrange
            string value = "gzip;";

            // Act
            bool parsed = HttpEncoding.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }
    }
}