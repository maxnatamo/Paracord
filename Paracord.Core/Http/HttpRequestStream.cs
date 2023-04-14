using System.Net.Sockets;

namespace Paracord.Core.Http
{
    public class HttpRequestStream : Stream
    {
        /// <summary>
        /// The internal <see cref="Stream" />-instance for handling TCP-sockets.
        /// </summary>
        protected Stream Stream { get; set; }

        /// <summary>
        /// Indicates whether the current stream supports reading.
        /// </summary>
        public override bool CanRead
        {
            get => true;
        }

        /// <summary>
        /// Indicates whether the current stream supports writing.
        /// </summary>
        public override bool CanWrite
        {
            get => false;
        }

        /// <summary>
        /// Indicates whether the current stream supports seeking.
        /// </summary>
        public override bool CanSeek
        {
            get => false;
        }

        /// <summary>
        /// The length in bytes of the stream.
        /// </summary>
        public override Int64 Length
        {
            get => throw new NotSupportedException("HttpRequestStream does not support Length_get");
        }

        /// <summary>
        /// The current position into the stream in bytes.
        /// </summary>
        public override Int64 Position
        {
            get => throw new NotSupportedException("HttpRequestStream does not support Position_get");
            set => throw new NotSupportedException("HttpRequestStream does not support Position_set");
        }

        /// <summary>
        /// Initialize a new <see cref="HttpRequestStream" />-instance with the specified <see cref="Stream" />-instance.
        /// </summary>
        /// <param name="stream">The parent <see cref="Stream" />-instance.</param>
        public HttpRequestStream(Stream stream)
        {
            this.Stream = stream;
        }

        /// <summary>
        /// Clears all buffers from the stream.
        /// </summary>
        public override void Flush()
        { }

        /// <summary>
        /// Clears all buffers from the stream.
        /// </summary>
        public override Task FlushAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;

        /// <summary>
        /// Read bytes from the stream and advance the position within the stream
        /// forward by the amount of bytes read.
        /// </summary>
        /// <param name="buffer">The output buffer for the resulting bytes read.</param>
        /// <param name="offset">The zero-based offset into the <paramref name="buffer" /> to begin storing bytes.</param>
        /// <param name="size">The maximum amount of bytes to read from the stream.</param>
        /// <returns>The total amount of bytes read into the buffer.</returns>
        public override int Read(byte[] buffer, int offset, int size)
            => this.Stream.Read(buffer, offset, size);

        /// <summary>
        /// Sets the position within the stream.
        /// </summary>
        /// <param name="offset">The relative offset to the <paramref name="origin" /> parameter in bytes.</param>
        /// <param name="origin">The reference point for the <paramref name="offset" /> parameter.</param>
        /// <returns>The new position within the stream.</returns>
        public override long Seek(long offset, SeekOrigin origin)
            => throw new NotSupportedException("HttpRequestStream does not support Seek");

        /// <summary>
        /// Sets the length of the stream.
        /// </summary>
        /// <param name="value">The new desired length of the stream in bytes.</param>
        public override void SetLength(long value)
            => throw new NotSupportedException("HttpRequestStream does not support SetLength");

        /// <summary>
        /// Write bytes from the buffer and advance the position within the stream
        /// forward by the amount of bytes written.
        /// </summary>
        /// <param name="buffer">
        /// The input buffer to read bytes from.
        /// This method copies <paramref name="size" /> bytes at <paramref name="offset" /> offset from the buffer into the stream.
        /// </param>
        /// <param name="offset">The zero-based offset into the <paramref name="buffer" /> to begin reading bytes.</param>
        /// <param name="size">The maximum amount of bytes to write from the stream.</param>
        public override void Write(byte[] buffer, int offset, int size)
            => throw new NotSupportedException("HttpRequestStream does not support Write");
    }
}