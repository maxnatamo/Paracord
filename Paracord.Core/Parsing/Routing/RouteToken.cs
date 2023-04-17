namespace Paracord.Core.Parsing.Routing
{
    public readonly struct RouteToken
    {
        /// <summary>
        /// the type of token.
        /// </summary>
        public readonly RouteTokenType Type;

        /// <summary>
        /// The actual value of the token.
        /// Some tokens, ie. symbols, have their value set to their respective symbol.
        /// </summary>
        public readonly string Value;

        /// <summary>
        /// The zero-indexed start of the token, relative to the string being parsed.
        /// </summary>
        public readonly int Start;

        /// <summary>
        /// The zero-indexed end of the token, relative to the string being parsed.
        /// </summary>
        public readonly int End;

        public RouteToken(RouteTokenType type, string value, int start, int end)
        {
            this.Type = type;
            this.Value = value;
            this.Start = start;
            this.End = end;
        }

        public RouteToken(RouteTokenType type, int start, int end)
        {
            this.Type = type;
            this.Value = string.Empty;
            this.Start = start;
            this.End = end;
        }

        /// <summary>
        /// Returns the string-representation of the token.
        /// </summary>
        public override string ToString() => HasTypeValue()
            ? $"{this.TypeDescription()} => \"{this.Value}\""
            : $"{this.TypeDescription()}";

        /// <summary>
        /// Indicates whether this type of token has an information <see cref="Value" /> value.
        /// </summary>
        private bool HasTypeValue() =>
            this.Type == RouteTokenType.NAME ||
            this.Type == RouteTokenType.UNKNOWN;

        /// <summary>
        /// Returns a description of the tokens type.
        /// </summary>
        private string TypeDescription() => this.Type switch
        {
            RouteTokenType.EOF => "EOF",
            RouteTokenType.EQUAL => "=",
            RouteTokenType.BRACE_LEFT => "{",
            RouteTokenType.BRACE_RIGHT => "}",
            RouteTokenType.NAME => "Name",
            RouteTokenType.UNKNOWN => "Unknown",

            _ => throw new InvalidDataException(this.Type.ToString())
        };
    }
}