using System.IO.Compression;

namespace Paracord.Core.Compression
{
    public class BrotliCompressionProvider : ICompressionProvider
    {
        /// <inheritdoc />
        /// <remarks>
        /// Expands to <c>br</c>.
        /// </remarks>
        public string AcceptedEncoding => "br";

        public byte[] Compress(Stream inputStream)
        {
            using var resultStream = new MemoryStream();
            using var compressionStream = new BrotliStream(resultStream, CompressionMode.Compress);

            inputStream.Seek(0, SeekOrigin.Begin);
            inputStream.CopyTo(compressionStream);
            compressionStream.Close();

            return resultStream.ToArray();
        }

        public byte[] Decompress(Stream inputStream)
        {
            using var resultStream = new MemoryStream();
            using var compressionStream = new BrotliStream(inputStream, CompressionMode.Decompress);

            inputStream.Seek(0, SeekOrigin.Begin);
            compressionStream.CopyTo(resultStream);
            compressionStream.Close();

            return resultStream.ToArray();
        }
    }
}