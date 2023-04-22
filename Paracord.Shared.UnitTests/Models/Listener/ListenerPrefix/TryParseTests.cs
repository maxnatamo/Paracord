using Paracord.Shared.Models.Listener;

namespace Paracord.Shared.UnitTests.Listener.ListenerPrefixTests
{
    public class TryParseTests
    {
        [Fact]
        public void TryParse_ReturnsFalse_GivenEmptyString()
        {
            // Arrange
            string value = string.Empty;

            // Act
            bool parsed = ListenerPrefix.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParse_ReturnsFalse_GivenNull()
        {
            // Arrange
            string value = null!;

            // Act
            bool parsed = ListenerPrefix.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParse_ReturnsFalse_GivenLongString()
        {
            // Arrange
            string value = string.Join("", Enumerable.Repeat('a', 300));

            // Act
            bool parsed = ListenerPrefix.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParse_ReturnsTrue_GivenAddress()
        {
            // Arrange
            string value = "127.0.0.1";

            // Act
            bool parsed = ListenerPrefix.TryParse(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result?.Secure.Should().Be(false);
            result?.Address.Should().Be("127.0.0.1");
            result?.Port.Should().Be(80);
        }

        [Fact]
        public void TryParse_ReturnsTrue_GivenAddressAndInsecureProtocol()
        {
            // Arrange
            string value = "http://127.0.0.1";

            // Act
            bool parsed = ListenerPrefix.TryParse(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result?.Secure.Should().Be(false);
            result?.Address.Should().Be("127.0.0.1");
            result?.Port.Should().Be(80);
        }

        [Fact]
        public void TryParse_ReturnsTrue_GivenAddressAndSecureProtocol()
        {
            // Arrange
            string value = "https://127.0.0.1";

            // Act
            bool parsed = ListenerPrefix.TryParse(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result?.Secure.Should().Be(true);
            result?.Address.Should().Be("127.0.0.1");
            result?.Port.Should().Be(80);
        }

        [Fact]
        public void TryParse_ReturnsFalse_GivenProtocol()
        {
            // Arrange
            string value = "https://";

            // Act
            bool parsed = ListenerPrefix.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParse_ReturnsTrue_GivenAddressAndPort()
        {
            // Arrange
            string value = "0.0.0.0:32000";

            // Act
            bool parsed = ListenerPrefix.TryParse(value, out var result);

            // Assert
            parsed.Should().BeTrue();
            result?.Address.Should().Be("0.0.0.0");
            result?.Port.Should().Be(32000);
        }

        [Fact]
        public void TryParse_ReturnsFalse_GivenPortNumberAboveLimit()
        {
            // Arrange
            string value = "0.0.0.0:96000";

            // Act
            bool parsed = ListenerPrefix.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }
    }
}