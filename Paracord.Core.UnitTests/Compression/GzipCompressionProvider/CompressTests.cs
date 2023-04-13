using System.Text;

using Paracord.Core.Compression;

namespace Paracord.Core.Tests.Compression.GzipCompressionProviderTests
{
    public class CompressTests
    {
        [Fact]
        public void CompressReturnsEmptyArrayGivenEmptyStream()
        {
            // Arrange
            MemoryStream inputStream = new MemoryStream();

            // Act
            byte[] compressed = new GzipCompressionProvider().Compress(inputStream);

            // Assert
            compressed.Length.Should().Be(0);
        }

        [Fact]
        public void CompressReturnsCompressedStringGivenStreamWithStringContent()
        {
            // Arrange
            string content = "This is a test string!";

            MemoryStream inputStream = new MemoryStream(Encoding.ASCII.GetBytes(content));

            // Act
            byte[] compressed = new GzipCompressionProvider().Compress(inputStream);

            // Assert
            compressed.Should().NotBeEmpty();
            compressed.Should().HaveCount(40);
            compressed.Should().BeEquivalentTo(new byte[]
            {
                0x1F, 0x8B, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03,
                0x0B, 0xC9, 0xC8, 0x2C, 0x56, 0x00, 0xA2, 0x44, 0x85, 0x92,
                0xD4, 0xE2, 0x12, 0x85, 0xE2, 0x92, 0xA2, 0xCC, 0xBC, 0x74,
                0x45, 0x00, 0xC5, 0xAC, 0x03, 0xAD, 0x16, 0x00, 0x00, 0x00,
            });
        }
    }
}