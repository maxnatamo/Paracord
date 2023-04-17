using Paracord.Core.Controller;
using Paracord.Core.Parsing.Routing;
using Paracord.Shared.Exceptions;

namespace Paracord.Core.UnitTests.Parsing.Routing.RouteParserTests
{
    public class ParseTests
    {
        [Fact]
        public void ParseThrowsUnexpectedTokenExceptionGivenEmptyString()
        {
            // Arrange
            string route = string.Empty;

            // Act
            Action act = () => RouteParser.Parse(route);

            // Assert
            act.Should().Throw<UnexpectedTokenException>();
        }

        [Fact]
        public void ParseReturnsConstantRouteSegmentGivenAlphabeticalWord()
        {
            // Arrange
            string route = "Controller";

            // Act
            ControllerRouteSegment segment = RouteParser.Parse(route);

            // Assert
            segment.Name.Should().Be("Controller");
            segment.Type.Should().Be(ControllerRouteSegmentType.Constant);
            segment.Default.Should().BeNull();
        }

        [Fact]
        public void ParseReturnsConstantRouteSegmentGivenAlphanumericWord()
        {
            // Arrange
            string route = "Co123ler";

            // Act
            ControllerRouteSegment segment = RouteParser.Parse(route);

            // Assert
            segment.Name.Should().Be("Co123ler");
            segment.Type.Should().Be(ControllerRouteSegmentType.Constant);
            segment.Default.Should().BeNull();
        }

        [Fact]
        public void ParseThrowsUnexpectedTokenExceptionGivenEnclosedAlphanumericWord()
        {
            // Arrange
            string route = "{co123ler}";

            // Act
            Action act = () => RouteParser.Parse(route);

            // Assert
            act.Should().Throw<UnexpectedTokenException>();
        }

        [Fact]
        public void ParseThrowsUnexpectedTokenExceptionGivenAlphabeticalWordWithEqualSign()
        {
            // Arrange
            string route = "Controller=";

            // Act
            Action act = () => RouteParser.Parse(route);

            // Assert
            act.Should().Throw<UnexpectedTokenException>();
        }

        [Fact]
        public void ParseReturnsVariableRouteSegmentGivenEnclosedValue()
        {
            // Arrange
            string route = "{controller}";

            // Act
            ControllerRouteSegment segment = RouteParser.Parse(route);

            // Assert
            segment.Name.Should().Be("controller");
            segment.Type.Should().Be(ControllerRouteSegmentType.Variable);
            segment.Default.Should().BeNull();
        }

        [Fact]
        public void ParseReturnsVariableRouteSegmentGivenEnclosedValueWithDefaultValue()
        {
            // Arrange
            string route = "{controller=index}";

            // Act
            ControllerRouteSegment segment = RouteParser.Parse(route);

            // Assert
            segment.Name.Should().Be("controller");
            segment.Type.Should().Be(ControllerRouteSegmentType.Variable);
            segment.Default.Should().Be("index");
        }

        [Fact]
        public void ParseReturnsVariableRouteSegmentGivenEnclosedValueWithAlphanumericDefaultValue()
        {
            // Arrange
            string route = "{controller=index1}";

            // Act
            ControllerRouteSegment segment = RouteParser.Parse(route);

            // Assert
            segment.Name.Should().Be("controller");
            segment.Type.Should().Be(ControllerRouteSegmentType.Variable);
            segment.Default.Should().Be("index1");
        }
    }
}