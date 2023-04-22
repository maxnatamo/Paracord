using Paracord.Core.Controller;
using Paracord.Core.Parsing.Routing;
using Paracord.Shared.Exceptions;

namespace Paracord.Core.UnitTests.Parsing.Routing.RouteParserTests
{
    public class ParseTests
    {
        [Fact]
        public void Parse_ReturnsNoSegments_GivenEmptyString()
        {
            // Arrange
            string route = string.Empty;

            // Act
            List<ControllerRouteSegment> segments = new RouteParser().Parse(route);

            // Assert
            segments.Should().BeEmpty();
        }

        [Fact]
        public void Parse_ReturnsConstantRouteSegment_GivenAlphabeticalWord()
        {
            // Arrange
            string route = "Controller";

            // Act
            List<ControllerRouteSegment> segments = new RouteParser().Parse(route);

            // Assert
            segments.Should().HaveCount(1);
            segments[0].Name.Should().Be("Controller");
            segments[0].Type.Should().Be(ControllerRouteSegmentType.Constant);
            segments[0].Default.Should().BeNull();
        }

        [Fact]
        public void Parse_ReturnsConstantRouteSegment_GivenAlphanumericWord()
        {
            // Arrange
            string route = "Co123ler";

            // Act
            List<ControllerRouteSegment> segments = new RouteParser().Parse(route);

            // Assert
            segments.Should().HaveCount(1);
            segments[0].Name.Should().Be("Co123ler");
            segments[0].Type.Should().Be(ControllerRouteSegmentType.Constant);
            segments[0].Default.Should().BeNull();
        }

        [Fact]
        public void Parse_ThrowsUnexpectedTokenException_GivenEnclosedAlphanumericWord()
        {
            // Arrange
            string route = "{co123ler}";

            // Act
            Action act = () => new RouteParser().Parse(route);

            // Assert
            act.Should().Throw<UnexpectedTokenException>();
        }

        [Fact]
        public void Parse_ThrowsUnexpectedTokenException_GivenAlphabeticalWordWithEqualSign()
        {
            // Arrange
            string route = "Controller=";

            // Act
            Action act = () => new RouteParser().Parse(route);

            // Assert
            act.Should().Throw<UnexpectedTokenException>();
        }

        [Fact]
        public void Parse_ThrowsUnexpectedTokenException_GivenBracesWithoutContent()
        {
            // Arrange
            string route = "{}";

            // Act
            Action act = () => new RouteParser().Parse(route);

            // Assert
            act.Should().Throw<UnexpectedTokenException>();
        }

        [Fact]
        public void Parse_ReturnsVariableRouteSegment_GivenEnclosedValue()
        {
            // Arrange
            string route = "{controller}";

            // Act
            List<ControllerRouteSegment> segments = new RouteParser().Parse(route);

            // Assert
            segments.Should().HaveCount(1);
            segments[0].Name.Should().Be("controller");
            segments[0].Type.Should().Be(ControllerRouteSegmentType.Variable);
            segments[0].Default.Should().BeNull();
        }

        [Fact]
        public void Parse_ReturnsVariableRouteSegment_GivenEnclosedValueWithDefaultValue()
        {
            // Arrange
            string route = "{controller=index}";

            // Act
            List<ControllerRouteSegment> segments = new RouteParser().Parse(route);

            // Assert
            segments.Should().HaveCount(1);
            segments[0].Name.Should().Be("controller");
            segments[0].Type.Should().Be(ControllerRouteSegmentType.Variable);
            segments[0].Default.Should().Be("index");
        }

        [Fact]
        public void Parse_ReturnsVariableRouteSegment_GivenEnclosedValueWithAlphanumericDefaultValue()
        {
            // Arrange
            string route = "{controller=index1}";

            // Act
            List<ControllerRouteSegment> segments = new RouteParser().Parse(route);

            // Assert
            segments.Should().HaveCount(1);
            segments[0].Name.Should().Be("controller");
            segments[0].Type.Should().Be(ControllerRouteSegmentType.Variable);
            segments[0].Default.Should().Be("index1");
        }

        [Fact]
        public void Parse_ReturnsConstantRouteSegments_GivenSeparatedWords()
        {
            // Arrange
            string route = "controller/action";

            // Act
            List<ControllerRouteSegment> segments = new RouteParser().Parse(route);

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
        public void Parse_ReturnsVariableRouteSegments_GivenSeparatedEnclosedWords()
        {
            // Arrange
            string route = "{controller}/{action}";

            // Act
            List<ControllerRouteSegment> segments = new RouteParser().Parse(route);

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
        public void Parse_ReturnsVariableRouteSegments_GivenSeparatedEnclosedWordsWithDefaultValue()
        {
            // Arrange
            string route = "{controller}/{action=Index}";

            // Act
            List<ControllerRouteSegment> segments = new RouteParser().Parse(route);

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