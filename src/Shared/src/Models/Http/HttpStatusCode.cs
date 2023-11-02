namespace Paracord.Shared.Models.Http
{
    /// <summary>
    /// <para>
    /// Native definitions for HTTP status codes, as defined in RFC9110.
    /// </para>
    /// <para>
    /// The first digit of the status-code indicates the class of response:
    /// <list type="bullet">
    ///     <item>
    ///         <term>1xx</term>
    ///         <description>(Informational): The request was recieved, continuing process.</description>
    ///     </item>
    ///     <item>
    ///         <term>2xx</term>
    ///         <description>(Successful): The request was successfully received, understood and accepted.</description>
    ///     </item>
    ///     <item>
    ///         <term>3xx</term>
    ///         <description>(Redirection): Further action is needed before the request can be completed.</description>
    ///     </item>
    ///     <item>
    ///         <term>4xx</term>
    ///         <description>(Client error): The request is malformed or not able to be completed.</description>
    ///     </item>
    ///     <item>
    ///         <term>5xx</term>
    ///         <description>(Server error): The server failed to complete and/or process the request.</description>
    ///     </item>
    /// </list>
    /// </para>
    /// </summary>
    /// <seealso href="https://httpwg.org/specs/rfc9110.html#status.codes">RFC9110 specification.</seealso>
    public enum HttpStatusCode
    {
        /**
         * Informational: The request was recieved, continuing process.
         */

        Continue = 100,
        SwitchingProtocols = 101,
        Processing = 102,

        /**
         * Successful: The request was successfully received, understood and accepted.
         */

        Ok = 200,
        Created = 201,
        Accepted = 202,
        NonAuthoritativeInformation = 203,
        NoContent = 204,
        ResetContent = 205,
        PartialContent = 206,
        MultiStatus = 207,
        AlreadyReported = 208,
        IMUsed = 226,

        /**
         * Redirection: Further action is needed before the request can be completed.
         */

        MultipleChoices = 300,
        MovedPermanently = 301,
        Found = 302,
        SeeOther = 303,
        NotModified = 304,
        TemporaryRedirect = 307,
        PermanentRedirect = 308,

        /**
         * Client error: The request is malformed or not able to be completed.
         */

        BadRequest = 400,
        Unauthorized = 401,
        PaymentRequired = 402,
        Forbidden = 403,
        NotFound = 404,
        MethodNotAllowed = 405,
        NotAcceptable = 406,
        ProxyAuthenticationRequired = 407,
        RequestTimeout = 408,
        Conflict = 409,
        Gone = 410,
        LengthRequired = 411,
        PreconditionFailed = 412,
        ContentTooLarge = 413,
        URITooLong = 414,
        UnsupportedMediaType = 415,
        RangeNotSatisfiable = 416,
        ExpectationFailed = 417,
        Teapot = 418,
        MisdirectedRequest = 421,
        UnprocessableContent = 422,
        Locked = 423,
        FailedDependency = 424,
        TooEarly = 425,
        UpgradeRequired = 426,
        PreconditionRequired = 428,
        TooManyRequests = 429,
        RequestHeaderFieldsTooLarge = 431,
        UnavailableForLegalReasons = 451,

        /**
         * Server error: The server failed to complete and/or process the request.
         */

        InternalServerError = 500,
        NotImplemented = 501,
        BadGateway = 502,
        ServiceUnavailable = 503,
        GatewayTimeout = 504,
        HttpVersionNotSupported = 505,
        VariantAlsoNegotiates = 506,
        InsufficientStorage = 507,
        LoopDetected = 508,
        NotExtended = 510,
        NetworkAuthenticationRequired = 511,
    }
}