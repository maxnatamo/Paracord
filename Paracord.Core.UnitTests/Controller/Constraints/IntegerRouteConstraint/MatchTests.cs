using Paracord.Core.Controller.Constraints;

namespace Paracord.Core.UnitTests.Controller.Constraints.IntegerRouteConstraintTests
{
    public class MatchTests
    {
        [Fact]
        public void MatchReturnsFalseGivenEmptyString()
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
        public void MatchReturnsFalseGivenString()
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
        public void IntegerRouteConstraint()
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
        public void MatchReturnsFalseGivenIntegerValueWithDecimalPoint()
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
        public void MatchReturnsTrueGivenNegativeNumber()
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
        public void MatchReturnsFalseGivenGivenHugeNumber()
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