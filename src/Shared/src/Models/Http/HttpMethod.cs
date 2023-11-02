namespace Paracord.Shared.Models.Http
{
    /// <summary>
    /// Native definitions for HTTP requests methods, also known as HTTP verbs.
    /// While custom methods are technically functional, no such mechanisms are implemented in the specification.
    /// </summary>
    /// <seealso href="https://httpwg.org/specs/rfc9110.html#method.definitions">RFC9110 specification.</seealso>
    [Flags]
    public enum HttpMethod
    {
        /// <summary>
        /// The <c>GET</c> method is meant to request specific resource(s) from the target.
        /// </summary>
        /// <seealso href="https://httpwg.org/specs/rfc9110.html#GET">Relavant RFC9110 section.</seealso>
        GET = (1 << 0),

        /// <summary>
        /// The <c>HEAD</c> method is identical to the GET method, but omits the content body and only sends the headers.
        /// </summary>
        /// <seealso href="https://httpwg.org/specs/rfc9110.html#HEAD">Relavant RFC9110 section.</seealso>
        HEAD = (1 << 1),

        /// <summary>
        /// The <c>POST</c> method is meant to submit specific resource(s) the the target, causing a state-change.
        /// </summary>
        /// <seealso href="https://httpwg.org/specs/rfc9110.html#POST">Relavant RFC9110 section.</seealso>
        POST = (1 << 2),

        /// <summary>
        /// The <c>PUT</c> method is meant to replace all target resources from the target with the request payload.
        /// </summary>
        /// <seealso href="https://httpwg.org/specs/rfc9110.html#PUT">Relavant RFC9110 section.</seealso>
        PUT = (1 << 3),

        /// <summary>
        /// The <c>DELETE</c> method is meant to delete specific resource(s) from the target.
        /// </summary>
        /// <seealso href="https://httpwg.org/specs/rfc9110.html#DELETE">Relavant RFC9110 section.</seealso>
        DELETE = (1 << 4),

        /// <summary>
        /// The <c>CONNECT</c> method is meant to establish a connection and/or tunnel with the target.
        /// </summary>
        /// <seealso href="https://httpwg.org/specs/rfc9110.html#CONNECT">Relavant RFC9110 section.</seealso>
        CONNECT = (1 << 5),

        /// <summary>
        /// The <c>OPTIONS</c> method is meant to describe communication options with the target.
        /// </summary>
        /// <seealso href="https://httpwg.org/specs/rfc9110.html#OPTIONS">Relavant RFC9110 section.</seealso>
        OPTIONS = (1 << 6),

        /// <summary>
        /// The <c>TRACE</c> method is meant for debugging and/or loop-back test to the target resource.
        /// </summary>
        /// <seealso href="https://httpwg.org/specs/rfc9110.html#TRACE">Relavant RFC9110 section.</seealso>
        TRACE = (1 << 7),

        /// <summary>
        /// The <c>PATCH</c> method is meant to apply partial updates to the target resource(s).
        /// </summary>
        /// <seealso href="https://httpwg.org/specs/rfc9110.html#PATCH">Relavant RFC9110 section.</seealso>
        PATCH = (1 << 8),

        /// <summary>
        /// All other <see cref="HttpMethod" />-enums.
        /// </summary>
        All = GET | POST | PUT | DELETE | CONNECT | OPTIONS | TRACE | PATCH,
    }
}