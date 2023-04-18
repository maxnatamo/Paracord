using Paracord.Core.Controller;

namespace Paracord.Core.UnitTests.Controller.ControllerRouteTests
{
    public class MatchTests
    {
        [Fact]
        public void MatchReturnsNonSuccessfulMatchGivenEmptyStringWithConstantRoute()
        {
            // Arrange
            string requestPath = string.Empty;
            ControllerRoute route = ControllerRoute.Parse("/", "index");

            // Act
            ControllerRouteMatch match = route.Match(requestPath);

            // Assert
            match.Success.Should().BeFalse();
        }

        [Fact]
        public void MatchReturnsSuccessfulMatchGivenUppercaseStringWithConstantRoute()
        {
            // Arrange
            string requestPath = "INDEX";
            ControllerRoute route = ControllerRoute.Parse("index", "");

            // Act
            ControllerRouteMatch match = route.Match(requestPath);

            // Assert
            match.Success.Should().BeTrue();
        }

        [Fact]
        public void MatchReturnsNonSuccessfulMatchGivenEmptyStringWithVariableRoute()
        {
            // Arrange
            string requestPath = string.Empty;
            ControllerRoute route = ControllerRoute.Parse("{controller}", "");

            // Act
            ControllerRouteMatch match = route.Match(requestPath);

            // Assert
            match.Success.Should().BeFalse();
        }

        [Fact]
        public void MatchReturnsSuccessfulMatchGivenStringWithVariableRoute()
        {
            // Arrange
            string requestPath = "index";
            ControllerRoute route = ControllerRoute.Parse("{controller}", "");

            // Act
            ControllerRouteMatch match = route.Match(requestPath);

            // Assert
            match.Success.Should().BeTrue();
            match.Parameters.Should().HaveCount(1);
            match.Parameters["controller"].Should().Be("index");
        }

        [Fact]
        public void MatchReturnsSuccessfulMatchGivenEmptyStringWithVariableRouteWithDefault()
        {
            // Arrange
            string requestPath = string.Empty;
            ControllerRoute route = ControllerRoute.Parse("{controller=index}", "");

            // Act
            ControllerRouteMatch match = route.Match(requestPath);

            // Assert
            match.Success.Should().BeTrue();
            match.Parameters.Should().HaveCount(1);
            match.Parameters["controller"].Should().Be("index");
        }

        [Fact]
        public void MatchReturnsSuccessfulMatchGivenStringWithVariableRouteWithDefault()
        {
            // Arrange
            string requestPath = "dashboard";
            ControllerRoute route = ControllerRoute.Parse("{controller=index}", "");

            // Act
            ControllerRouteMatch match = route.Match(requestPath);

            // Assert
            match.Success.Should().BeTrue();
            match.Parameters.Should().HaveCount(1);
            match.Parameters["controller"].Should().Be("dashboard");
        }
    }
}