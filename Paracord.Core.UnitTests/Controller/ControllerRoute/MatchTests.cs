using LightInject;
using Paracord.Core.Application;
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
        public void Match_ReturnsNonSuccessfulMatch_GivenEmptyStringWithConstantRoute()
        {
            // Arrange
            HttpRequest request = this.MakeRequest(string.Empty);
            WebApplication application = new WebApplication(new ServiceContainer(), new WebApplicationEnvironment(), new WebApplicationOptions());
            ControllerRoute route = ControllerRoute.Parse("/", "index");

            // Act
            ControllerRouteMatch match = route.Match(application, request);

            // Assert
            match.Success.Should().BeFalse();
        }

        [Fact]
        public void Match_ReturnsSuccessfulMatch_GivenUppercaseStringWithConstantRoute()
        {
            // Arrange
            HttpRequest request = this.MakeRequest("index");
            WebApplication application = new WebApplication(new ServiceContainer(), new WebApplicationEnvironment(), new WebApplicationOptions());
            ControllerRoute route = ControllerRoute.Parse("index", "");

            // Act
            ControllerRouteMatch match = route.Match(application, request);

            // Assert
            match.Success.Should().BeTrue();
        }

        [Fact]
        public void Match_ReturnsNonSuccessfulMatch_GivenEmptyStringWithVariableRoute()
        {
            // Arrange
            HttpRequest request = this.MakeRequest(string.Empty);
            WebApplication application = new WebApplication(new ServiceContainer(), new WebApplicationEnvironment(), new WebApplicationOptions());
            ControllerRoute route = ControllerRoute.Parse("{controller}", "");

            // Act
            ControllerRouteMatch match = route.Match(application, request);

            // Assert
            match.Success.Should().BeFalse();
        }

        [Fact]
        public void Match_ReturnsSuccessfulMatch_GivenStringWithVariableRoute()
        {
            // Arrange
            HttpRequest request = this.MakeRequest("index");
            WebApplication application = new WebApplication(new ServiceContainer(), new WebApplicationEnvironment(), new WebApplicationOptions());
            ControllerRoute route = ControllerRoute.Parse("{controller}", "");

            // Act
            ControllerRouteMatch match = route.Match(application, request);

            // Assert
            match.Success.Should().BeTrue();
            match.Parameters.Should().HaveCount(1);
            match.Parameters["controller"].Should().Be("index");
        }

        [Fact]
        public void Match_ReturnsSuccessfulMatch_GivenEmptyStringWithVariableRouteWithDefault()
        {
            // Arrange
            HttpRequest request = this.MakeRequest(string.Empty);
            WebApplication application = new WebApplication(new ServiceContainer(), new WebApplicationEnvironment(), new WebApplicationOptions());
            ControllerRoute route = ControllerRoute.Parse("{controller=index}", "");

            // Act
            ControllerRouteMatch match = route.Match(application, request);

            // Assert
            match.Success.Should().BeTrue();
            match.Parameters.Should().HaveCount(1);
            match.Parameters["controller"].Should().Be("index");
        }

        [Fact]
        public void Match_ReturnsSuccessfulMatch_GivenStringWithVariableRouteWithDefault()
        {
            // Arrange
            HttpRequest request = this.MakeRequest("dashboard");
            WebApplication application = new WebApplication(new ServiceContainer(), new WebApplicationEnvironment(), new WebApplicationOptions());
            ControllerRoute route = ControllerRoute.Parse("{controller=index}", "");

            // Act
            ControllerRouteMatch match = route.Match(application, request);

            // Assert
            match.Success.Should().BeTrue();
            match.Parameters.Should().HaveCount(1);
            match.Parameters["controller"].Should().Be("dashboard");
        }
    }
}