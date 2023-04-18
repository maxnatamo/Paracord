using Paracord.Core.Controller;
using Paracord.Core.Http;

using HttpMethod = Paracord.Shared.Models.Http.HttpMethod;

namespace Paracord.Core.UnitTests.Controller.ControllerRouteTests
{
    public class MatchTests
    {
        private HttpRequest MakeRequest(string path, HttpMethod method = HttpMethod.GET)
            => new HttpRequest
            {
                Target = new HttpTarget
                {
                    PathSegments = new string[] { path }
                },
                Method = method
            };

        [Fact]
        public void MatchReturnsNonSuccessfulMatchGivenEmptyStringWithConstantRoute()
        {
            // Arrange
            HttpRequest request = this.MakeRequest(string.Empty);
            ControllerRoute route = ControllerRoute.Parse("/", "index");

            // Act
            ControllerRouteMatch match = route.Match(request);

            // Assert
            match.Success.Should().BeFalse();
        }

        [Fact]
        public void MatchReturnsSuccessfulMatchGivenUppercaseStringWithConstantRoute()
        {
            // Arrange
            HttpRequest request = this.MakeRequest("index");
            ControllerRoute route = ControllerRoute.Parse("index", "");

            // Act
            ControllerRouteMatch match = route.Match(request);

            // Assert
            match.Success.Should().BeTrue();
        }

        [Fact]
        public void MatchReturnsNonSuccessfulMatchGivenEmptyStringWithVariableRoute()
        {
            // Arrange
            HttpRequest request = this.MakeRequest(string.Empty);
            ControllerRoute route = ControllerRoute.Parse("{controller}", "");

            // Act
            ControllerRouteMatch match = route.Match(request);

            // Assert
            match.Success.Should().BeFalse();
        }

        [Fact]
        public void MatchReturnsSuccessfulMatchGivenStringWithVariableRoute()
        {
            // Arrange
            HttpRequest request = this.MakeRequest("index");
            ControllerRoute route = ControllerRoute.Parse("{controller}", "");

            // Act
            ControllerRouteMatch match = route.Match(request);

            // Assert
            match.Success.Should().BeTrue();
            match.Parameters.Should().HaveCount(1);
            match.Parameters["controller"].Should().Be("index");
        }

        [Fact]
        public void MatchReturnsSuccessfulMatchGivenEmptyStringWithVariableRouteWithDefault()
        {
            // Arrange
            HttpRequest request = this.MakeRequest(string.Empty);
            ControllerRoute route = ControllerRoute.Parse("{controller=index}", "");

            // Act
            ControllerRouteMatch match = route.Match(request);

            // Assert
            match.Success.Should().BeTrue();
            match.Parameters.Should().HaveCount(1);
            match.Parameters["controller"].Should().Be("index");
        }

        [Fact]
        public void MatchReturnsSuccessfulMatchGivenStringWithVariableRouteWithDefault()
        {
            // Arrange
            HttpRequest request = this.MakeRequest("dashboard");
            ControllerRoute route = ControllerRoute.Parse("{controller=index}", "");

            // Act
            ControllerRouteMatch match = route.Match(request);

            // Assert
            match.Success.Should().BeTrue();
            match.Parameters.Should().HaveCount(1);
            match.Parameters["controller"].Should().Be("dashboard");
        }
    }
}