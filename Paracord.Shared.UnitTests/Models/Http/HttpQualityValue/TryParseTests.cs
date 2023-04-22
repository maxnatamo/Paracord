using Paracord.Shared.Models.Http;

namespace Paracord.Shared.UnitTests.Http.HttpQualityValueTests
{
    public class TryParseTests
    {
        [Fact]
        public void TryParse_ReturnsFalse_GivenEmptyString()
        {
            // Arrange
            string value = string.Empty;

            // Act
            bool parsed = HttpQualityValue.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParse_ReturnsFalse_GivenNull()
        {
            // Arrange
            string value = null!;

            // Act
            bool parsed = HttpQualityValue.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParse_ReturnsFalse_GivenByteSequence()
        {
            // Arrange
            string value = "\u0000\u0000";

            // Act
            bool parsed = HttpQualityValue.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParse_ReturnsFalse_GivenLongString()
        {
            // Arrange
            string value = string.Join("", Enumerable.Repeat('a', 300));

            // Act
            bool parsed = HttpQualityValue.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParse_ReturnsTrue_GivenValueWithoutWeight()
        {
            // Arrange
            string value = "value";

            // Act
            bool parsed = HttpQualityValue.TryParse(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result?.Value.Should().Be("value");
            result?.Weight.Should().Be(1.0f);
        }

        [Fact]
        public void TryParse_ReturnsFalse_GivenValueWithSemicolon()
        {
            // Arrange
            string value = "value;";

            // Act
            bool parsed = HttpQualityValue.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParse_ReturnsFalse_GivenOnlyWeight()
        {
            // Arrange
            string value = "q=1.0";

            // Act
            bool parsed = HttpQualityValue.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParse_ReturnsFalse_GivenOnlyWeightWithSemicolon()
        {
            // Arrange
            string value = ";q=1.0";

            // Act
            bool parsed = HttpQualityValue.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParse_ReturnsFalse_GivenWeightWithNonFloatValue()
        {
            // Arrange
            string value = "value;q=s";

            // Act
            bool parsed = HttpQualityValue.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParse_ReturnsFalse_GivenValueWithIntegerWeight()
        {
            // Arrange
            string value = "value;q=1";

            // Act
            bool parsed = HttpQualityValue.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParse_ReturnsFalse_GivenValueWithoutDecimal()
        {
            // Arrange
            string value = "value;q=1.";

            // Act
            bool parsed = HttpQualityValue.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParse_ReturnsTrue_GivenValueWithValidWeight()
        {
            // Arrange
            string value = "value;q=0.7";

            // Act
            bool parsed = HttpQualityValue.TryParse(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result?.Value.Should().Be("value");
            result?.Weight.Should().Be(0.7f);
        }

        [Fact]
        public void TryParse_ReturnsFalse_GivenValueWithWeightAboveOne()
        {
            // Arrange
            string value = "value;q=1.1";

            // Act
            bool parsed = HttpQualityValue.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParse_ReturnsFalse_GivenValueWithNegativeWeight()
        {
            // Arrange
            string value = "value;q=-0.1";

            // Act
            bool parsed = HttpQualityValue.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }
    }
}