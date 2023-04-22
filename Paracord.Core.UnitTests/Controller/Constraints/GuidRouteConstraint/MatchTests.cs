using Paracord.Core.Controller.Constraints;

namespace Paracord.Core.UnitTests.Controller.Constraints.GuidRouteConstraintTests
{
    public class MatchTests
    {
        [Fact]
        public void MatchReturnsFalseGivenEmptyString()
        {
            // Arrange
            string value = string.Empty;

            // Act
            bool parsed = new GuidRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeFalse();
            result.Should().BeNull();
        }

        [Fact]
        public void MatchReturnsFalseGivenLetter()
        {
            // Arrange
            string value = "a";

            // Act
            bool parsed = new GuidRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeFalse();
            result.Should().BeNull();
        }

        [Fact]
        public void MatchReturnsTrueGivenGuidWith32Digits()
        {
            // Arrange
            string value = "00000000000000000000000000000000";

            // Act
            bool parsed = new GuidRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result.Should().NotBeNull();
            result.Should().BeOfType<Guid>();
            result.As<Guid>().Should().Be("00000000000000000000000000000000");
        }

        [Fact]
        public void MatchReturnsTrueGivenGuidWith32DigitsWithHyphens()
        {
            // Arrange
            string value = "00000000-0000-0000-0000-000000000000";

            // Act
            bool parsed = new GuidRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result.Should().NotBeNull();
            result.Should().BeOfType<Guid>();
            result.As<Guid>().Should().Be("00000000-0000-0000-0000-000000000000");
        }

        [Fact]
        public void MatchReturnsFalseGivenGuidWith32DigitsWithHyphensAndBraces()
        {
            // Arrange
            string value = "{00000000-0000-0000-0000-000000000000}";

            // Act
            bool parsed = new GuidRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeFalse();
            result.Should().BeNull();
        }

        [Fact]
        public void MatchReturnsFalseGivenGuidWith32DigitsWithHyphensAndParentheses()
        {
            // Arrange
            string value = "(00000000-0000-0000-0000-000000000000)";

            // Act
            bool parsed = new GuidRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeFalse();
            result.Should().BeNull();
        }

        [Fact]
        public void MatchReturnsFalseGivenGuidWith32DigitsInHexadecimal()
        {
            // Arrange
            string value = "{0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}}";

            // Act
            bool parsed = new GuidRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeFalse();
            result.Should().BeNull();
        }
    }
}