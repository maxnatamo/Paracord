using System.Security.Cryptography.X509Certificates;
using Paracord.Shared.Models.Listener;

namespace Paracord.Core.Application
{
    /// <summary>
    /// Runtime options for <see cref="WebApplication"/>-instances.
    /// </summary>
    public class WebApplicationOptions
    {
        /// <summary>
        /// An optional SSL certificate to use for secure HTTP connections. HTTPS cannot be enabled, if null.
        /// </summary>
        public X509Certificate2? SslCertificate { get; set; } = null;

        /// <summary>
        /// The address/prefixes to listen on.
        /// </summary>
        public readonly ListenerPrefixCollection Prefixes = new ListenerPrefixCollection();
    }
}