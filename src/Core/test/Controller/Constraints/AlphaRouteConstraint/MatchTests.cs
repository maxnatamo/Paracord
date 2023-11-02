using Paracord.Core.Controller.Constraints;

namespace Paracord.Core.Tests.Controller.Constraints.AlphaRouteConstraintTests
{
    public class MatchTests
    {
        [Fact]
        public void Match_ReturnsFalse_GivenEmptyString()
        {
            // Arrange
            string value = string.Empty;

            // Act
            bool parsed = new AlphaRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeFalse();
            result.Should().BeNull();
        }

        [Fact]
        public void Match_ReturnsTrue_GivenLowercaseLetter()
        {
            // Arrange
            string value = "a";

            // Act
            bool parsed = new AlphaRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result.Should().NotBeNull();
            result.Should().BeOfType<string>();
            result.As<string>().Should().Be("a");
        }

        [Fact]
        public void Match_ReturnsTrue_GivenUppercaseLetter()
        {
            // Arrange
            string value = "A";

            // Act
            bool parsed = new AlphaRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result.Should().NotBeNull();
            result.Should().BeOfType<string>();
            result.As<string>().Should().Be("A");
        }

        [Fact]
        public void Match_ReturnsFalse_GivenNumericString()
        {
            // Arrange
            string value = "1";

            // Act
            bool parsed = new AlphaRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeFalse();
            result.Should().BeNull();
        }
    }
}