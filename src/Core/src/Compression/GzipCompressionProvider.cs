using System.IO.Compression;

namespace Paracord.Core.Compression
{
    public class GzipCompressionProvider : ICompressionProvider
    {
        /// <inheritdoc />
        /// <remarks>
        /// Expands to <c>gzip</c>.
        /// </remarks>
        public string AcceptedEncoding => "gzip";

        public byte[] Compress(Stream inputStream)
        {
            using var resultStream = new MemoryStream();
            using var compressionStream = new GZipStream(resultStream, CompressionMode.Compress);

            inputStream.Seek(0, SeekOrigin.Begin);
            inputStream.CopyTo(compressionStream);
            compressionStream.Close();

            return resultStream.ToArray();
        }

        public byte[] Decompress(Stream inputStream)
        {
            using var resultStream = new MemoryStream();
            using var compressionStream = new GZipStream(inputStream, CompressionMode.Decompress);

            inputStream.Seek(0, SeekOrigin.Begin);
            compressionStream.CopyTo(resultStream);
            compressionStream.Close();

            return resultStream.ToArray();
        }
    }
}