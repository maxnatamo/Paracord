namespace Paracord.Core.Compression
{
    public interface ICompressionProvider
    {
        /// <summary>
        /// Defines which encodings are accepted by the current provider, defined in the <c>Accept-Encoding</c> and <c>Content-Encoding</c> headers.
        /// </summary>
        public string AcceptedEncoding { get; }

        /// <summary>
        /// Compress the specified byte-buffer using the current compression provider and return it.
        /// </summary>
        /// <remarks>
        /// The position of the <paramref name="inputStream"/> is reset before being read, but it is not restored.
        /// </remarks>
        /// <param name="inputStream">An input <see cref="Stream"/> containing the data to compress.</param>
        /// <returns>A byte-array containing the compressed data.</returns>
        public byte[] Compress(Stream inputStream);

        /// <summary>
        /// Decompress the specified byte-buffer using the current compression provider and return it.
        /// </summary>
        /// <remarks>
        /// The position of the <paramref name="inputStream"/> is reset before being read, but it is not restored.
        /// </remarks>
        /// <param name="inputStream">An input <see cref="Stream"/> containing the data to compress.</param>
        /// <returns>A byte-array containing the decompressed data.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the number of bytes read exceeds the limit.</exception>
        public byte[] Decompress(Stream inputStream);
    }
}