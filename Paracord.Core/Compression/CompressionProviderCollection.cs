using System.Collections.ObjectModel;

namespace Paracord.Core.Compression
{
    /// <summary>
    /// A collection of <see cref="ICompressionProvider" />-instances, available to a parent service.
    /// </summary>
    public class CompressionProviderCollection : Collection<ICompressionProvider>
    {
        /// <summary>
        /// Adds a type representing a <see cref="ICompressionProvider" />-instance, which can be used for compression and/or decompression.
        /// </summary>
        /// <typeparam name="T">Type representing a <see cref="ICompressionProvider" />-instance.</typeparam>
        public void Add<T>() where T : ICompressionProvider, new()
            => this.Add(new T());

        /// <summary>
        /// Find a <see cref="ICompressionProvider" />-instance that supports the specified encoding format and return.
        /// </summary>
        /// <param name="encoding">The encoding format to query for.</param>
        /// <returns>The associated compression provider, if found. Otherwise, null.</returns>
        public ICompressionProvider? FindProvider(string encoding)
        {
            IEnumerable<ICompressionProvider> providers = this.Where(v => v.AcceptedEncoding == encoding);

            if(providers.Any())
            {
                return providers.First();
            }

            return null;
        }
    }
}