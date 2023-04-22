using Paracord.Core.Controller.Constraints;

namespace Paracord.Core.UnitTests.Controller.Constraints.FloatRouteConstraintTests
{
    public class MatchTests
    {
        [Fact]
        public void MatchReturnsFalseGivenEmptyString()
        {
            // Arrange
            string value = string.Empty;

            // Act
            bool parsed = new FloatRouteConstraint().Match(value, out var result);

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
            bool parsed = new FloatRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeFalse();
            result.Should().BeNull();
        }

        [Fact]
        public void MatchReturnsTrueGivenIntegerValue()
        {
            // Arrange
            string value = "1";

            // Act
            bool parsed = new FloatRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result.Should().NotBeNull();
            result.Should().BeOfType<Single>();
            result.As<Single>().Should().Be(1);
        }

        [Fact]
        public void MatchReturnsTrueGivenIntegerValueWithDecimalPoint()
        {
            // Arrange
            string value = "1.";

            // Act
            bool parsed = new FloatRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result.Should().NotBeNull();
            result.Should().BeOfType<Single>();
            result.As<Single>().Should().Be(1);
        }

        [Fact]
        public void MatchReturnsTrueGivenFloatingPointNumber()
        {
            // Arrange
            string value = "1.123";

            // Act
            bool parsed = new FloatRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result.Should().NotBeNull();
            result.Should().BeOfType<Single>();
            result.As<Single>().Should().Be(1.123f);
        }

        [Fact]
        public void MatchReturnsFalseGivenNumberWithThousandsSeparator()
        {
            // Arrange
            string value = "1,000";

            // Act
            bool parsed = new FloatRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeFalse();
            result.Should().BeNull();
        }

        [Fact]
        public void MatchReturnsFalseGivenNumberWithCurrency()
        {
            // Arrange
            string value = "$1,000";

            // Act
            bool parsed = new FloatRouteConstraint().Match(value, out var result);

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
            bool parsed = new FloatRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result.Should().NotBeNull();
            result.Should().BeOfType<Single>();
            result.As<Single>().Should().Be(-1);
        }

        [Fact]
        public void MatchReturnsFalseGivenNumberInExponentialNotation()
        {
            // Arrange
            string value = "1.24E3";

            // Act
            bool parsed = new FloatRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeFalse();
            result.Should().BeNull();
        }
    }
}