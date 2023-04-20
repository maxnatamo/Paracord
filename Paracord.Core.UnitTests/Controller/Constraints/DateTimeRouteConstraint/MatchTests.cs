using FluentAssertions.Extensions;
using Paracord.Core.Controller.Constraints;

namespace Paracord.Core.UnitTests.Controller.Constraints.DateTimeRouteConstraintTests
{
    public class MatchTests
    {
        [Fact]
        public void MatchReturnsFalseGivenEmptyString()
        {
            // Arrange
            string value = string.Empty;

            // Act
            bool parsed = new DateTimeRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeFalse();
            result.Should().BeNull();
        }

        [Fact]
        public void MatchReturnsTrueGivenLongDateString()
        {
            // Arrange
            string value = new DateTime(2007, 1, 9, 9, 41, 12).ToLongDateString();

            // Act
            bool parsed = new DateTimeRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result.Should().NotBeNull();
            result.Should().BeOfType<DateTime>();
            result.As<DateTime>().Should().HaveYear(2007);
            result.As<DateTime>().Should().HaveMonth(1);
            result.As<DateTime>().Should().HaveDay(9);
        }

        [Fact]
        public void MatchReturnsTrueGivenLongTimeString()
        {
            // Arrange
            string value = new DateTime(2007, 1, 9, 9, 41, 12).ToLongTimeString();

            // Act
            bool parsed = new DateTimeRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result.Should().NotBeNull();
            result.Should().BeOfType<DateTime>();
            result.As<DateTime>().Should().HaveHour(9);
            result.As<DateTime>().Should().HaveMinute(41);
            result.As<DateTime>().Should().HaveSecond(12);
        }

        [Fact]
        public void MatchReturnsTrueGivenShortDateString()
        {
            // Arrange
            string value = new DateTime(2007, 1, 9, 9, 41, 12).ToShortDateString();

            // Act
            bool parsed = new DateTimeRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result.Should().NotBeNull();
            result.Should().BeOfType<DateTime>();
            result.As<DateTime>().Should().HaveYear(2007);
            result.As<DateTime>().Should().HaveMonth(1);
            result.As<DateTime>().Should().HaveDay(9);
        }

        [Fact]
        public void MatchReturnsTrueGivenShortTimeString()
        {
            // Arrange
            string value = new DateTime(2007, 1, 9, 9, 41, 12).ToShortTimeString();

            // Act
            bool parsed = new DateTimeRouteConstraint().Match(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result.Should().NotBeNull();
            result.Should().BeOfType<DateTime>();
            result.As<DateTime>().Should().HaveHour(9);
            result.As<DateTime>().Should().HaveMinute(41);
            result.As<DateTime>().Should().HaveSecond(0); // ShortTimeString doesn't include seconds.
        }
    }
}