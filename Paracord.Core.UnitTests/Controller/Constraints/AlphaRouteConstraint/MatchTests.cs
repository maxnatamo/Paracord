using Paracord.Core.Controller.Constraints;

namespace Paracord.Core.UnitTests.Controller.Constraints.AlphaRouteConstraintTests
{
    public class MatchTests
    {
        [Fact]
        public void MatchReturnsFalseGivenEmptyString()
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
        public void MatchReturnsTrueGivenLowercaseLetter()
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
        public void MatchReturnsTrueGivenUppercaseLetter()
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
        public void MatchReturnsFalseGivenNumericString()
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