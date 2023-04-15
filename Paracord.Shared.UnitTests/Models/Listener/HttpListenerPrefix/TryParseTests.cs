using Paracord.Shared.Models.Listener;

namespace Paracord.Shared.Tests.Listener.ListenerPrefixTests
{
    public class TryParseTests
    {
        [Fact]
        public void TryParseReturnsFalseGivenEmptyString()
        {
            // Arrange
            string value = string.Empty;

            // Act
            bool parsed = ListenerPrefix.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParseReturnsFalseGivenNull()
        {
            // Arrange
            string value = null!;

            // Act
            bool parsed = ListenerPrefix.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParseReturnsFalseGivenLongString()
        {
            // Arrange
            string value = string.Join("", Enumerable.Repeat('a', 300));

            // Act
            bool parsed = ListenerPrefix.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParseReturnsTrueGivenAddress()
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
        public void TryParseReturnsTrueGivenAddressAndInsecureProtocol()
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
        public void TryParseReturnsTrueGivenAddressAndSecureProtocol()
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
        public void TryParseReturnsFalseGivenProtocol()
        {
            // Arrange
            string value = "https://";

            // Act
            bool parsed = ListenerPrefix.TryParse(value, out var _);

            // Assert
            parsed.Should().BeFalse();
        }

        [Fact]
        public void TryParseReturnsTrueGivenAddressAndPort()
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
        public void TryParseReturnsFalseGivenPortNumberAboveLimit()
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