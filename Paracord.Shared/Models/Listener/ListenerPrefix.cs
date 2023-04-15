using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Paracord.Shared.Models.Listener
{
    /// <summary>
    /// Definition of a prefix (e.g. <c>http://127.0.0.1:32000</c>, <c>ssl://0.0.0.0:80</c>, etc.),
    /// including IP address, port and whether it's secure.
    /// </summary>
    public class ListenerPrefix
    {
        /// <summary>
        /// List of protocols utilizing TLS/SSL encryption.
        /// </summary>
        public static readonly IEnumerable<string> SecureProtocols = new List<string>
        {
            "https",
            "ssl",
            "tls"
        };

        /// <summary>
        /// The IP address of the prefix. Defaults to <c>127.0.0.1</c>.
        /// </summary>
        public string Address { get; private set; } = "127.0.0.1";

        /// <summary>
        /// The port number of the prefix. Defaults to <c>80</c>.
        /// </summary>
        public uint Port { get; private set; } = 80;

        /// <summary>
        /// The protocol of the prefix. Defaults to <c>http</c>.
        /// </summary>
        public string Protocol { get; private set; } = "http";

        /// <summary>
        /// Whether the prefix is using a secure protocol, ie. HTTPS.
        /// </summary>
        public bool Secure
        {
            get => ListenerPrefix.SecureProtocols.Contains(this.Protocol);
        }

        /// <summary>
        /// Initialize an empty <see cref="ListenerPrefix" />-instance with default values.
        /// </summary>
        public ListenerPrefix()
        {

        }

        /// <summary>
        /// Initialize an <see cref="ListenerPrefix" />-instance with the specified values.
        /// </summary>
        /// <param name="address">The IP-address of the instance.</param>
        /// <param name="port">The port number of the instance.</param>
        /// <param name="protocol">The protocol of the instance.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the port number is above 65535.</exception>
        public ListenerPrefix(string address, uint port = 80, string protocol = "http")
        {
            if(port > 65535)
            {
                throw new ArgumentOutOfRangeException(nameof(port), "Port number cannot be above 65535");
            }

            this.Address = address;
            this.Port = port;
            this.Protocol = protocol;
        }

        /// <summary>
        /// Parse an address (e.g. <c>http://localhost:32000</c>, <c>0.0.0.0:80</c>, etc.) to a native <c>ListenerPrefix</c>-instance.
        /// </summary>
        /// <param name="address">The address to parse. This may include protocol, IP-address and/or port.</param>
        /// <param name="result">The resulting <c>ListenerPrefix</c>-instance, if the method returns true. Otherwise, false.</param>
        /// <returns>True, if the string was successfully parsed. Otherwise, false.</returns>
        public static bool TryParse(string address, [NotNullWhen(true)] out ListenerPrefix? result)
        {
            result = null;

            if(string.IsNullOrEmpty(address) || address.Length > 256)
            {
                return false;
            }

            address = address.Trim();

            Regex pattern = new Regex(@"^((?<protocol>[a-zA-Z]+):\/\/)?(?<address>(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}))(:(?<port>(\d+)))?$");
            Match match = pattern.Match(address);

            if(!match.Success)
            {
                return false;
            }

            result = new ListenerPrefix();
            result.Address = match.Groups["address"].Value;

            if(match.Groups["protocol"].Success)
            {
                result.Protocol = match.Groups["protocol"].Value;
            }

            if(match.Groups["port"].Success)
            {
                if(!uint.TryParse(match.Groups["port"].Value, out var port))
                {
                    result = null;
                    return false;
                }

                if(port > 65535)
                {
                    result = null;
                    return false;
                }

                result.Port = port;
            }

            return true;
        }

        /// <returns>The parsed <c>ListenerPrefix</c>-instance.</returns>
        /// <exception cref="FormatException">Thrown if the parsing failed.</exception>
        /// <inheritdoc cref="ListenerPrefix.TryParse" />
        public static ListenerPrefix Parse(string address)
        {
            if(ListenerPrefix.TryParse(address, out var result))
            {
                return result;
            }
            throw new FormatException("The supplied prefix is improperly formatted.");
        }
    }
}