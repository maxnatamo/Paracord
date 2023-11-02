using Paracord.Core.Controller.Constraints;

namespace Paracord.Core.Tests.Controller.Constraints.IntegerRouteConstraintTests
{
    public class MatchTests
    {
        [Fact]
        public void Match_ReturnsFalse_GivenEmptyString()
        {
            // Arrange
            string value = string.Empty;

            // Act
            bool parsed = new IntegerRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeFalse();
            result.Should().BeNull();
        }

        [Fact]
        public void Match_ReturnsFalse_GivenString()
        {
            // Arrange
            string value = "a";

            // Act
            bool parsed = new IntegerRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeFalse();
            result.Should().BeNull();
        }

        [Fact]
        public void Match_ReturnsFalse_GivenIntegerValue()
        {
            // Arrange
            string value = "1";

            // Act
            bool parsed = new IntegerRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result.Should().NotBeNull();
            result.Should().BeOfType<Int32>();
            result.As<Int32>().Should().Be(1);
        }

        [Fact]
        public void Match_ReturnsFalse_GivenIntegerValueWithDecimalPoint()
        {
            // Arrange
            string value = "1.";

            // Act
            bool parsed = new IntegerRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeFalse();
            result.Should().BeNull();
        }

        [Fact]
        public void Match_ReturnsTrue_GivenNegativeNumber()
        {
            // Arrange
            string value = "-1";

            // Act
            bool parsed = new IntegerRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result.Should().NotBeNull();
            result.Should().BeOfType<Int32>();
            result.As<Int32>().Should().Be(-1);
        }

        [Fact]
        public void Match_ReturnsFalse_GivenGivenHugeNumber()
        {
            // Arrange
            string value = Int64.MaxValue.ToString();

            // Act
            bool parsed = new IntegerRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeFalse();
            result.Should().BeNull();
        }
    }
}