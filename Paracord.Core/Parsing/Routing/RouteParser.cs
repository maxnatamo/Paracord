using Paracord.Core.Controller;
using Paracord.Shared.Exceptions;

namespace Paracord.Core.Parsing.Routing
{
    public static partial class RouteParser
    {
        /// <summary>
        /// The <see cref="RouteTokenizer" />-instance to transform inputs into tokens.
        /// </summary>
        internal static RouteTokenizer Tokenizer = new RouteTokenizer();

        /// <summary>
        /// The current position into the token-list
        /// </summary>
        internal static int CurrentTokenIndex { get; set; } = 0;

        /// <summary>
        /// The currently-processing token
        /// </summary>
        internal static RouteToken CurrentToken = new RouteToken();

        /// <summary>
        /// Parse the specified route into a list of <see cref="ControllerRouteSegment" />-instances.
        /// </summary>
        /// <param name="route">The route to parse.</param>
        /// <returns>List of <see cref="ControllerRouteSegment" />-instances.</returns>
        /// <exception cref="MissingBraceException">Thrown when the number of braces isn't even.</exception>
        /// <exception cref="UnexpectedTokenException">Thrown when an unexpected token was found.</exception>
        public static List<ControllerRouteSegment> Parse(string route)
        {
            RouteParser.Tokenizer.SetSource(route);
            RouteParser.CurrentTokenIndex = 0;
            RouteParser.CurrentToken = RouteParser.Tokenizer.GetNextToken();

            List<ControllerRouteSegment> segments = new List<ControllerRouteSegment>();

            segments.Add(RouteParser.ParseDefinition());

            while(RouteParser.Peek(RouteTokenType.SLASH))
            {
                RouteParser.Skip();
                segments.Add(RouteParser.ParseDefinition());
            }

            return segments;
        }

        /// <summary>
        /// Parse the current source into a <see cref="ControllerRouteSegment" />-instance.
        /// </summary>
        /// <returns>A non-null <see cref="ControllerRouteSegment" />, if a definition was found. Otherwise, null.</returns>
        /// <exception cref="UnexpectedTokenException">Thrown when an unexpected token was found.</exception>
        internal static ControllerRouteSegment ParseDefinition()
        {
            if(RouteParser.Peek(RouteTokenType.NAME))
            {
                return RouteParser.ParseConstantRoute();
            }

            if(RouteParser.Peek(RouteTokenType.BRACE_LEFT))
            {
                return RouteParser.ParseVariableRoute();
            }

            throw UnexpectedToken();
        }

        /// <summary>
        /// Parse the current source into a constant <see cref="ControllerRouteSegment" />-instance.
        /// </summary>
        /// <returns>The parsed <see cref="ControllerRouteSegment" />-object.</returns>
        /// <exception cref="UnexpectedTokenException">Thrown when an unexpected token was found.</exception>
        internal static ControllerRouteSegment ParseConstantRoute()
        {
            RouteParser.Expect(RouteTokenType.NAME);

            ControllerRouteSegment segment = new ControllerRouteSegment();
            segment.Type = ControllerRouteSegmentType.Constant;
            segment.Name = RouteParser.CurrentToken.Value;
            segment.Default = null;

            RouteParser.Skip();
            RouteParser.Expect(RouteTokenType.EOF, RouteTokenType.SLASH);

            return segment;
        }

        /// <summary>
        /// Parse the current source into a variable <see cref="ControllerRouteSegment" />-instance.
        /// </summary>
        /// <returns>The parsed <see cref="ControllerRouteSegment" />-object.</returns>
        /// <exception cref="UnexpectedTokenException">Thrown when an unexpected token was found.</exception>
        internal static ControllerRouteSegment ParseVariableRoute()
        {
            RouteParser.Expect(RouteTokenType.BRACE_LEFT);
            RouteParser.Skip();

            RouteParser.Expect(RouteTokenType.NAME);

            ControllerRouteSegment segment = new ControllerRouteSegment();
            segment.Type = ControllerRouteSegmentType.Variable;
            segment.Name = RouteParser.CurrentToken.Value;

            if(segment.Name.Any(v => v >= '0' && v <= '9'))
            {
                throw UnexpectedToken();
            }

            RouteParser.Skip();

            if(RouteParser.Peek(RouteTokenType.EQUAL))
            {
                RouteParser.Skip();
                segment.Default = RouteParser.CurrentToken.Value;

                RouteParser.Skip();
            }

            RouteParser.Expect(RouteTokenType.BRACE_RIGHT);
            RouteParser.Skip();

            RouteParser.Expect(RouteTokenType.EOF, RouteTokenType.SLASH);

            return segment;
        }

        /// <summary>
        /// Peek the type of the current token.
        /// </summary>
        /// <param name="type">The token-type to peek for.</param>
        /// <returns>True, if the type matches the current token. Otherwise, false.</returns>
        internal static bool Peek(RouteTokenType type)
        {
            return RouteParser.CurrentToken.Type == type;
        }

        /// <summary>
        /// Peek the type of the current token and throw if it doesn't match any of the specified types.
        /// </summary>
        /// <param name="types">The token-types to assert for.</param>
        /// <exception cref="UnexpectedTokenException">Thrown if the assertion fails.</exception>
        internal static void Expect(params RouteTokenType[] types)
        {
            if(!types.Any(v => RouteParser.Peek(v)))
            {
                throw UnexpectedToken();
            }
        }

        /// <summary>
        /// Progress to the next token.
        /// </summary>
        /// <remarks>
        /// If the position points to the end of the file, nothing is done.
        /// </remarks>
        internal static void Skip()
        {
            RouteParser.CurrentTokenIndex++;
            RouteParser.CurrentToken = RouteParser.Tokenizer.GetNextToken();
        }

        /// <summary>
        /// Return <see cref="UnexpectedTokenException" />-object with location attached as description.
        /// </summary>
        /// <returns><see cref="UnexpectedTokenException" />-object.</returns>
        internal static Exception UnexpectedToken()
            => new UnexpectedTokenException(RouteParser.CurrentToken.Start);
    }
}