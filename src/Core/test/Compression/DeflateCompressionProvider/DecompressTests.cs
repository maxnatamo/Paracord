using System.Text;

using Paracord.Core.Compression;

namespace Paracord.Core.UnitTests.Compression.DeflateCompressionProviderTests
{
    public class DecompressTests
    {
        [Fact]
        public void Decompress_ReturnsEmptyArray_GivenEmptyStream()
        {
            // Arrange
            MemoryStream inputStream = new MemoryStream();

            // Act
            byte[] decompressed = new DeflateCompressionProvider().Decompress(inputStream);

            // Assert
            decompressed.Length.Should().Be(0);
        }

        [Fact]
        public void Decompress_ReturnsSameAsInput_GivenStringContent()
        {
            // Arrange
            string content = "This is a test string!";

            MemoryStream inputStream = new MemoryStream(Encoding.ASCII.GetBytes(content));

            // Act
            byte[] compressed = new DeflateCompressionProvider().Compress(new MemoryStream(Encoding.ASCII.GetBytes(content)));
            byte[] decompressed = new DeflateCompressionProvider().Decompress(new MemoryStream(compressed));

            // Assert
            Encoding.ASCII.GetString(decompressed.ToArray()).Should().Be(content);
        }
    }
}