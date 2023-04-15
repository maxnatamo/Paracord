using Paracord.Core.Http;

namespace Paracord.Core.UnitTests.Http.HttpTargetTests
{
    public class TryParseTests
    {
        [Fact]
        public void TryParseReturnsFalseGivenEmptyString()
        {
            // Arrange
            string value = string.Empty;

            // Act
            bool parsed = HttpTarget.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParseReturnsTrueGivenRootPath()
        {
            // Arrange
            string value = "/";

            // Act
            bool parsed = HttpTarget.TryParse(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result?.Path.Should().Be("/");
            result?.QueryParameters.Should().BeEmpty();
        }

        [Fact]
        public void TryParseReturnsTrueGivenNestedPath()
        {
            // Arrange
            string value = "/a/b/c";

            // Act
            bool parsed = HttpTarget.TryParse(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result?.Path.Should().Be("/a/b/c");
            result?.QueryParameters.Should().BeEmpty();
        }

        [Fact]
        public void TryParseReturnsFalseSingleQueryParameter()
        {
            // Arrange
            string value = "?a=c";

            // Act
            bool parsed = HttpTarget.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParseReturnsTrueGivenRootPathWithSingleQueryParameter()
        {
            // Arrange
            string value = "/?a=c";

            // Act
            bool parsed = HttpTarget.TryParse(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result?.Path.Should().Be("/");
            result?.QueryParameters.Should().NotBeEmpty();
            result?.QueryParameters.Should().ContainKey("a");
            result?.QueryParameters["a"].Should().Be("c");
        }

        [Fact]
        public void TryParseReturnsFalseGivenEmptyQueryName()
        {
            // Arrange
            string value = "?=c";

            // Act
            bool parsed = HttpTarget.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParseReturnsTrueWhenGivenQueryParametersWithHashtag()
        {
            // Arrange
            string value = "/?a=1#";

            // Act
            bool parsed = HttpTarget.TryParse(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result?.Should().NotBeNull();
            result?.QueryParameters.Should().ContainKey("a");
            result?.QueryParameters["a"].Should().Be("1");
            result?.QueryParameters["a"].Should().NotBe("1#");
        }
    }
}