namespace Paracord.Shared.Models.Http
{
    /// <summary>
    /// Native definitions for HTTP requests methods, also known as HTTP verbs.
    /// While custom methods are technically functional, no such mechanisms are implemented in the specification.
    /// </summary>
    /// <seealso href="https://httpwg.org/specs/rfc9110.html#method.definitions">RFC9110 specification.</seealso>
    public enum HttpMethod
    {
        /// <summary>
        /// The <c>GET</c> method is meant to request specific resource(s) from the target.
        /// </summary>
        /// <seealso href="https://httpwg.org/specs/rfc9110.html#GET">Relavant RFC9110 section.</seealso>
        GET,

        /// <summary>
        /// The <c>HEAD</c> method is identical to the GET method, but omits the content body and only sends the headers.
        /// </summary>
        /// <seealso href="https://httpwg.org/specs/rfc9110.html#HEAD">Relavant RFC9110 section.</seealso>
        HEAD,

        /// <summary>
        /// The <c>POST</c> method is meant to submit specific resource(s) the the target, causing a state-change.
        /// </summary>
        /// <seealso href="https://httpwg.org/specs/rfc9110.html#POST">Relavant RFC9110 section.</seealso>
        POST,

        /// <summary>
        /// The <c>PUT</c> method is meant to replace all target resources from the target with the request payload.
        /// </summary>
        /// <seealso href="https://httpwg.org/specs/rfc9110.html#PUT">Relavant RFC9110 section.</seealso>
        PUT,

        /// <summary>
        /// The <c>DELETE</c> method is meant to delete specific resource(s) from the target.
        /// </summary>
        /// <seealso href="https://httpwg.org/specs/rfc9110.html#DELETE">Relavant RFC9110 section.</seealso>
        DELETE,

        /// <summary>
        /// The <c>CONNECT</c> method is meant to establish a connection and/or tunnel with the target.
        /// </summary>
        /// <seealso href="https://httpwg.org/specs/rfc9110.html#CONNECT">Relavant RFC9110 section.</seealso>
        CONNECT,

        /// <summary>
        /// The <c>OPTIONS</c> method is meant to describe communication options with the target.
        /// </summary>
        /// <seealso href="https://httpwg.org/specs/rfc9110.html#OPTIONS">Relavant RFC9110 section.</seealso>
        OPTIONS,

        /// <summary>
        /// The <c>TRACE</c> method is meant for debugging and/or loop-back test to the target resource.
        /// </summary>
        /// <seealso href="https://httpwg.org/specs/rfc9110.html#TRACE">Relavant RFC9110 section.</seealso>
        TRACE,

        /// <summary>
        /// The <c>PATCH</c> method is meant to apply partial updates to the target resource(s).
        /// </summary>
        /// <seealso href="https://httpwg.org/specs/rfc9110.html#PATCH">Relavant RFC9110 section.</seealso>
        PATCH,
    }
}