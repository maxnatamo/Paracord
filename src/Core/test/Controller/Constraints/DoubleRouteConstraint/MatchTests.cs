using Paracord.Core.Controller.Constraints;

namespace Paracord.Core.Tests.Controller.Constraints.DoubleRouteConstraintTests
{
    public class MatchTests
    {
        [Fact]
        public void Match_ReturnsFalse_GivenEmptyString()
        {
            // Arrange
            string value = string.Empty;

            // Act
            bool parsed = new DoubleRouteConstraint().Match(value, out var result);

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
            bool parsed = new DoubleRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeFalse();
            result.Should().BeNull();
        }

        [Fact]
        public void Match_ReturnsTrue_GivenIntegerValue()
        {
            // Arrange
            string value = "1";

            // Act
            bool parsed = new DoubleRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result.Should().NotBeNull();
            result.Should().BeOfType<Double>();
            result.As<Double>().Should().Be(1);
        }

        [Fact]
        public void Match_ReturnsTrue_GivenIntegerValueWithDecimalPoint()
        {
            // Arrange
            string value = "1.";

            // Act
            bool parsed = new DoubleRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result.Should().NotBeNull();
            result.Should().BeOfType<Double>();
            result.As<Double>().Should().Be(1);
        }

        [Fact]
        public void Match_ReturnsTrue_GivenFloatingPointNumber()
        {
            // Arrange
            string value = "1.123";

            // Act
            bool parsed = new DoubleRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result.Should().NotBeNull();
            result.Should().BeOfType<Double>();
            result.As<Double>().Should().BeApproximately(1.123f, 0.001f);
        }

        [Fact]
        public void Match_ReturnsFalse_GivenNumberWithThousandsSeparator()
        {
            // Arrange
            string value = "1,000";

            // Act
            bool parsed = new DoubleRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeFalse();
            result.Should().BeNull();
        }

        [Fact]
        public void Match_ReturnsFalse_GivenNumberWithCurrency()
        {
            // Arrange
            string value = "$1,000";

            // Act
            bool parsed = new DoubleRouteConstraint().Match(value, out var result);

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
            bool parsed = new DoubleRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result.Should().NotBeNull();
            result.Should().BeOfType<Double>();
            result.As<Double>().Should().Be(-1);
        }

        [Fact]
        public void Match_ReturnsFalse_GivenNumberInExponentialNotation()
        {
            // Arrange
            string value = "1.24E3";

            // Act
            bool parsed = new DoubleRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeFalse();
            result.Should().BeNull();
        }
    }
}