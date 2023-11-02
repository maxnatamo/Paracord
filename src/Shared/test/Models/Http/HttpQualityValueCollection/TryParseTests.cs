using Paracord.Shared.Models.Http;

namespace Paracord.Shared.Tests.Http.HttpQualityValueCollectionTests
{
    public class TryParseTests
    {
        [Fact]
        public void TryParse_ReturnsFalse_GivenEmptyString()
        {
            // Arrange
            string value = string.Empty;

            // Act
            bool parsed = HttpQualityValueCollection.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParse_ReturnsFalse_GivenOnlyCommas()
        {
            // Arrange
            string value = ",,,,";

            // Act
            bool parsed = HttpQualityValueCollection.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParse_ReturnsFalse_GivenValidQualityValueWithComma()
        {
            // Arrange
            string value = "gzip;q=1.0,";

            // Act
            bool parsed = HttpQualityValueCollection.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParse_ReturnsTrue_GivenSingleValidQualityValue()
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