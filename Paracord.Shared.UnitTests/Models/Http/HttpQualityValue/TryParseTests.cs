using Paracord.Shared.Models.Http;

namespace Paracord.Shared.UnitTests.Http.HttpQualityValueTests
{
    public class TryParseTests
    {
        [Fact]
        public void TryParseReturnsFalseGivenEmptyString()
        {
            // Arrange
            string value = string.Empty;

            // Act
            bool parsed = HttpQualityValue.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParseReturnsFalseGivenNull()
        {
            // Arrange
            string value = null!;

            // Act
            bool parsed = HttpQualityValue.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParseReturnsFalseGivenByteSequence()
        {
            // Arrange
            string value = "\u0000\u0000";

            // Act
            bool parsed = HttpQualityValue.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParseReturnsFalseGivenLongString()
        {
            // Arrange
            string value = string.Join("", Enumerable.Repeat('a', 300));

            // Act
            bool parsed = HttpQualityValue.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParseReturnsTrueGivenValueWithoutWeight()
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
        public void TryParseReturnsFalseGivenValueWithSemicolon()
        {
            // Arrange
            string value = "value;";

            // Act
            bool parsed = HttpQualityValue.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParseReturnsFalseGivenOnlyWeight()
        {
            // Arrange
            string value = "q=1.0";

            // Act
            bool parsed = HttpQualityValue.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParseReturnsFalseGivenOnlyWeightWithSemicolon()
        {
            // Arrange
            string value = ";q=1.0";

            // Act
            bool parsed = HttpQualityValue.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParseReturnsFalseGivenWeightWithNonFloatValue()
        {
            // Arrange
            string value = "value;q=s";

            // Act
            bool parsed = HttpQualityValue.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParseReturnsFalseGivenValueWithIntegerWeight()
        {
            // Arrange
            string value = "value;q=1";

            // Act
            bool parsed = HttpQualityValue.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParseReturnsFalseGivenValueWithoutDecimal()
        {
            // Arrange
            string value = "value;q=1.";

            // Act
            bool parsed = HttpQualityValue.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParseReturnsTrueGivenValueWithValidWeight()
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
        public void TryParseReturnsFalseGivenValueWithWeightAboveOne()
        {
            // Arrange
            string value = "value;q=1.1";

            // Act
            bool parsed = HttpQualityValue.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParseReturnsFalseGivenValueWithNegativeWeight()
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