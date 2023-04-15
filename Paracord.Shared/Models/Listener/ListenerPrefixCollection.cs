using System.Collections.ObjectModel;

namespace Paracord.Shared.Models.Listener
{
    /// <summary>
    /// Collection of <see cref="ListenerPrefix" />-instances.
    /// </summary>
    public class ListenerPrefixCollection : Collection<ListenerPrefix>
    {
        /// <summary>
        /// Adds a new prefix to the collection, with the specified address.
        /// </summary>
        /// <param name="address">The address of the prefix-instance.</param>
        public void Add(string address)
        {
            this.Add(ListenerPrefix.Parse(address));
        }

        /// <summary>
        /// Adds a new prefix to the collection, with the specified values.
        /// </summary>
        /// <param name="address">The IP address of the prefix-instance.</param>
        /// <param name="port">The port of the prefix-instance.</param>
        /// <param name="protocol">The protocol of the prefix-instance.</param>
        public void Add(string address, uint port = 80, string protocol = "http")
        {
            this.Add(new ListenerPrefix(address, port, protocol));
        }
    }
}