using Paracord.Core.Controller.Constraints;

namespace Paracord.Core.UnitTests.Controller.Constraints.GuidRouteConstraintTests
{
    public class MatchTests
    {
        [Fact]
        public void Match_ReturnsFalse_GivenEmptyString()
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
        public void Match_ReturnsFalse_GivenLetter()
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
        public void Match_ReturnsTrue_GivenGuidWith32Digits()
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
        public void Match_ReturnsTrue_GivenGuidWith32DigitsWithHyphens()
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
        public void Match_ReturnsFalse_GivenGuidWith32DigitsWithHyphensAndBraces()
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
        public void Match_ReturnsFalse_GivenGuidWith32DigitsWithHyphensAndParentheses()
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
        public void Match_ReturnsFalse_GivenGuidWith32DigitsInHexadecimal()
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