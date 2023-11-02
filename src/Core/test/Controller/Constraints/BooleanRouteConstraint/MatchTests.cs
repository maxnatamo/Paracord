using Paracord.Core.Controller.Constraints;

namespace Paracord.Core.Tests.Controller.Constraints.BooleanRouteConstraintTests
{
    public class MatchTests
    {
        [Fact]
        public void Match_ReturnsFalse_GivenEmptyString()
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
        public void Match_ReturnsFalse_GivenString()
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
        public void Match_ReturnsTrue_GivenTrueString()
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
        public void Match_ReturnsTrue_GivenFalseString()
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
        public void Match_ReturnsTrue_GivenUppercaseTrueString()
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
        public void Match_ReturnsTrue_GivenUppercaseFalseString()
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