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
            List<ControllerRouteSegment> segments = RouteParser.Parse(route);

            // Assert
            segments.Should().HaveCount(1);
            segments[0].Name.Should().Be("Controller");
            segments[0].Type.Should().Be(ControllerRouteSegmentType.Constant);
            segments[0].Default.Should().BeNull();
        }

        [Fact]
        public void ParseReturnsConstantRouteSegmentGivenAlphanumericWord()
        {
            // Arrange
            string route = "Co123ler";

            // Act
            List<ControllerRouteSegment> segments = RouteParser.Parse(route);

            // Assert
            segments.Should().HaveCount(1);
            segments[0].Name.Should().Be("Co123ler");
            segments[0].Type.Should().Be(ControllerRouteSegmentType.Constant);
            segments[0].Default.Should().BeNull();
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
        public void ParseThrowsUnexpectedTokenExceptionGivenBracesWithoutContent()
        {
            // Arrange
            string route = "{}";

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
            List<ControllerRouteSegment> segments = RouteParser.Parse(route);

            // Assert
            segments.Should().HaveCount(1);
            segments[0].Name.Should().Be("controller");
            segments[0].Type.Should().Be(ControllerRouteSegmentType.Variable);
            segments[0].Default.Should().BeNull();
        }

        [Fact]
        public void ParseReturnsVariableRouteSegmentGivenEnclosedValueWithDefaultValue()
        {
            // Arrange
            string route = "{controller=index}";

            // Act
            List<ControllerRouteSegment> segments = RouteParser.Parse(route);

            // Assert
            segments.Should().HaveCount(1);
            segments[0].Name.Should().Be("controller");
            segments[0].Type.Should().Be(ControllerRouteSegmentType.Variable);
            segments[0].Default.Should().Be("index");
        }

        [Fact]
        public void ParseReturnsVariableRouteSegmentGivenEnclosedValueWithAlphanumericDefaultValue()
        {
            // Arrange
            string route = "{controller=index1}";

            // Act
            List<ControllerRouteSegment> segments = RouteParser.Parse(route);

            // Assert
            segments.Should().HaveCount(1);
            segments[0].Name.Should().Be("controller");
            segments[0].Type.Should().Be(ControllerRouteSegmentType.Variable);
            segments[0].Default.Should().Be("index1");
        }

        [Fact]
        public void ParseReturnsConstantRouteSegmentsGivenSeparatedWords()
        {
            // Arrange
            string route = "controller/action";

            // Act
            List<ControllerRouteSegment> segments = RouteParser.Parse(route);

            // Assert
            segments.Should().HaveCount(2);
            segments[0].Name.Should().Be("controller");
            segments[0].Type.Should().Be(ControllerRouteSegmentType.Constant);
            segments[0].Default.Should().BeNull();
            segments[1].Name.Should().Be("action");
            segments[1].Type.Should().Be(ControllerRouteSegmentType.Constant);
            segments[1].Default.Should().BeNull();
        }

        [Fact]
        public void ParseReturnsVariableRouteSegmentsGivenSeparatedEnclosedWords()
        {
            // Arrange
            string route = "{controller}/{action}";

            // Act
            List<ControllerRouteSegment> segments = RouteParser.Parse(route);

            // Assert
            segments.Should().HaveCount(2);
            segments[0].Name.Should().Be("controller");
            segments[0].Type.Should().Be(ControllerRouteSegmentType.Variable);
            segments[0].Default.Should().BeNull();
            segments[1].Name.Should().Be("action");
            segments[1].Type.Should().Be(ControllerRouteSegmentType.Variable);
            segments[1].Default.Should().BeNull();
        }

        [Fact]
        public void ParseReturnsVariableRouteSegmentsGivenSeparatedEnclosedWordsWithDefaultValue()
        {
            // Arrange
            string route = "{controller}/{action=Index}";

            // Act
            List<ControllerRouteSegment> segments = RouteParser.Parse(route);

            // Assert
            segments.Should().HaveCount(2);
            segments[0].Name.Should().Be("controller");
            segments[0].Type.Should().Be(ControllerRouteSegmentType.Variable);
            segments[0].Default.Should().BeNull();
            segments[1].Name.Should().Be("action");
            segments[1].Type.Should().Be(ControllerRouteSegmentType.Variable);
            segments[1].Default.Should().Be("Index");
        }
    }
}