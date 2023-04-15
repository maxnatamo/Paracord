using Paracord.Shared.Models.Http;

namespace Paracord.Shared.UnitTests.Http.HttpQualityValueCollectionTests
{
    public class TryParseTests
    {
        [Fact]
        public void TryParseReturnsFalseGivenEmptyString()
        {
            // Arrange
            string value = string.Empty;

            // Act
            bool parsed = HttpQualityValueCollection.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParseReturnsFalseGivenOnlyCommas()
        {
            // Arrange
            string value = ",,,,";

            // Act
            bool parsed = HttpQualityValueCollection.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParseReturnsFalseGivenValidQualityValueWithComma()
        {
            // Arrange
            string value = "gzip;q=1.0,";

            // Act
            bool parsed = HttpQualityValueCollection.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParseReturnsTrueGivenSingleValidQualityValue()
        {
            // Arrange
            string value = "gzip;q=1.0";

            // Act
            bool parsed = HttpQualityValueCollection.TryParse(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result?.Should().ContainSingle();
            result?[0].Value.Should().Be("gzip");
            result?[0].Weight.Should().Be(1.0f);
        }
    }
}