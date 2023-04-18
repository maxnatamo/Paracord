namespace Paracord.Core.Parsing.Routing
{
    public enum RouteTokenType
    {
        /// <summary>
        /// End-of-line
        /// </summary>
        EOF,

        /// <summary>
        /// Equal sign, =
        /// </summary>
        EQUAL,

        /// <summary>
        /// Forward slash, /
        /// </summary>
        SLASH,

        /// <summary>
        /// Open brace, {
        /// </summary>
        BRACE_LEFT,

        /// <summary>
        /// Close brace, }
        /// </summary>
        BRACE_RIGHT,

        /// <summary>
        /// A string value, without double-quotes.
        /// </summary>
        NAME,

        /// <summary>
        /// An unidentified and/or unknown value.
        /// </summary>
        UNKNOWN,
    }
}