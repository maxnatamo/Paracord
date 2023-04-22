using Paracord.Core.Http;

namespace Paracord.Core.UnitTests.Http.HttpTargetTests
{
    public class TryParseTests
    {
        [Fact]
        public void TryParse_ReturnsFalse_GivenEmptyString()
        {
            // Arrange
            string value = string.Empty;

            // Act
            bool parsed = HttpTarget.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParse_ReturnsTrue_GivenRootPath()
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
        public void TryParse_ReturnsTrue_GivenNestedPath()
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
        public void TryParse_ReturnsFalse_GivenSingleQueryParameter()
        {
            // Arrange
            string value = "?a=c";

            // Act
            bool parsed = HttpTarget.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParse_ReturnsTrue_GivenRootPathWithSingleQueryParameter()
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
        public void TryParse_ReturnsFalse_GivenEmptyQueryName()
        {
            // Arrange
            string value = "?=c";

            // Act
            bool parsed = HttpTarget.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParse_ReturnsTrueWhen_GivenQueryParametersWithHashtag()
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