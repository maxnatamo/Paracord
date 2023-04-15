using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Paracord.Shared.Models.Listener
{
    /// <summary>
    /// Definition of an HTTP prefix (e.g. <c>http://127.0.0.1:32000</c>, <c>0.0.0.0:80</c>, etc.),
    /// including IP address, port and whether it's secure.
    /// </summary>
    public class HttpListenerPrefix
    {
        /// <summary>
        /// The IP address of the prefix. Defaults to <c>127.0.0.1</c>.
        /// </summary>
        public string Address { get; private set; } = "127.0.0.1";

        /// <summary>
        /// The port number of the prefix. Defaults to <c>80</c>.
        /// </summary>
        public uint Port { get; private set; } = 80;

        /// <summary>
        /// Whether the prefix is using a secure protocol, ie. HTTPS. Defaults to <c>false</c>.
        /// </summary>
        public bool Secure { get; private set; } = false;

        /// <summary>
        /// Initialize an empty <see cref="HttpListenerPrefix" />-instance with default values.
        /// </summary>
        public HttpListenerPrefix()
        {

        }

        /// <summary>
        /// Initialize an <see cref="HttpListenerPrefix" />-instance with the specified values.
        /// </summary>
        /// <param name="address">The IP-address of the instance.</param>
        /// <param name="port">The port number of the instance.</param>
        /// <param name="secure">Whether the prefix should use a secure protocol (ie. HTTPS).</param>
        public HttpListenerPrefix(string address, uint port, bool secure = false)
        {
            this.Address = address;
            this.Port = port;
            this.Secure = secure;
        }

        /// <summary>
        /// Parse an address (e.g. <c>http://localhost:32000</c>, <c>0.0.0.0:80</c>, etc.) to a native <c>HttpListenerPrefix</c>-instance.
        /// </summary>
        /// <param name="address">The address to parse. This may include protocol, IP-address and/or port.</param>
        /// <param name="result">The resulting <c>HttpListenerPrefix</c>-instance, if the method returns true. Otherwise, false.</param>
        /// <returns>True, if the string was successfully parsed. Otherwise, false.</returns>
        public static bool TryParse(string address, [NotNullWhen(true)] out HttpListenerPrefix? result)
        {
            result = null;

            if(address.Length == 0 || address.Length > 256)
            {
                return false;
            }

            address = address.Trim();

            Regex pattern = new Regex(@"^((?<protocol>http(s)?):\/\/)?(?<address>(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}))(:(?<port>(\d+)))?$");
            Match match = pattern.Match(address);

            if(!match.Success)
            {
                return false;
            }

            result = new HttpListenerPrefix();
            result.Address = match.Groups["address"].Value;

            if(match.Groups.ContainsKey("protocol"))
            {
                result.Secure = match.Groups["protocol"].Value == "https";
            }

            if(match.Groups.ContainsKey("port"))
            {
                if(!uint.TryParse(match.Groups["port"].Value, out var port))
                {
                    result = null;
                    return false;
                }

                result.Port = port;
            }

            return true;
        }

        /// <returns>The parsed <c>HttpListenerPrefix</c>-instance.</returns>
        /// <exception cref="FormatException">Thrown if the parsing failed.</exception>
        /// <inheritdoc cref="HttpListenerPrefix.TryParse" />
        public static HttpListenerPrefix Parse(string address)
        {
            if(HttpListenerPrefix.TryParse(address, out var result))
            {
                return result;
            }
            throw new FormatException("The supplied HTTP prefix is improperly formatted.");
        }
    }
}