using Paracord.Core.Controller.Constraints;

namespace Paracord.Core.UnitTests.Controller.Constraints.BooleanRouteConstraintTests
{
    public class MatchTests
    {
        [Fact]
        public void MatchReturnsFalseGivenEmptyString()
        {
            // Arrange
            string value = string.Empty;

            // Act
            bool parsed = new BooleanRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeFalse();
            result.Should().BeNull();
        }

        [Fact]
        public void MatchReturnsFalseGivenString()
        {
            // Arrange
            string value = "A";

            // Act
            bool parsed = new BooleanRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeFalse();
            result.Should().BeNull();
        }

        [Fact]
        public void MatchReturnsTrueGivenTrueString()
        {
            // Arrange
            string value = "true";

            // Act
            bool parsed = new BooleanRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result.Should().NotBeNull();
            result.Should().BeOfType<bool>();
            result.As<bool>().Should().BeTrue();
        }

        [Fact]
        public void MatchReturnsTrueGivenFalseString()
        {
            // Arrange
            string value = "false";

            // Act
            bool parsed = new BooleanRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result.Should().NotBeNull();
            result.Should().BeOfType<bool>();
            result.As<bool>().Should().BeFalse();
        }

        [Fact]
        public void MatchReturnsTrueGivenUppercaseTrueString()
        {
            // Arrange
            string value = "TRUE";

            // Act
            bool parsed = new BooleanRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result.Should().NotBeNull();
            result.Should().BeOfType<bool>();
            result.As<bool>().Should().BeTrue();
        }

        [Fact]
        public void MatchReturnsTrueGivenUppercaseFalseString()
        {
            // Arrange
            string value = "FALSE";

            // Act
            bool parsed = new BooleanRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result.Should().NotBeNull();
            result.Should().BeOfType<bool>();
            result.As<bool>().Should().BeFalse();
        }
    }
}