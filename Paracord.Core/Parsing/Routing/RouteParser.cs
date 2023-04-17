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
        /// Parse the specified route into a <see cref="ControllerRouteSegment" />-instance.
        /// </summary>
        /// <param name="route">The route to parse</param>
        /// <returns>The parsed <see cref="ControllerRouteSegment" />-object.</returns>
        /// <exception cref="MissingBraceException">Thrown when the number of braces isn't even.</exception>
        /// <exception cref="UnexpectedTokenException">Thrown when an unexpected token was found.</exception>
        public static ControllerRouteSegment Parse(string route)
        {
            RouteParser.Tokenizer.SetSource(route);
            RouteParser.CurrentTokenIndex = 0;
            RouteParser.CurrentToken = RouteParser.Tokenizer.GetNextToken();

            ControllerRouteSegment segment = new ControllerRouteSegment();

            RouteParser.ParseDefinition(segment);

            return segment;
        }

        /// <summary>
        /// Parse the current source into a <see cref="ControllerRouteSegment" />-instance.
        /// </summary>
        /// <param name="segment">The <see cref="ControllerRouteSegment" />-instance to fill.</param>
        /// <exception cref="UnexpectedTokenException">Thrown when an unexpected token was found.</exception>
        public static void ParseDefinition(ControllerRouteSegment segment)
        {
            if(RouteParser.Peek(RouteTokenType.NAME))
            {
                RouteParser.ParseConstantRoute(segment);
                return;
            }

            if(RouteParser.Peek(RouteTokenType.BRACE_LEFT))
            {
                RouteParser.ParseVariableRoute(segment);
                return;
            }

            throw RouteParser.UnexpectedToken();
        }

        /// <summary>
        /// Parse the current source into a constant <see cref="ControllerRouteSegment" />-instance.
        /// </summary>
        /// <param name="segment">The <see cref="ControllerRouteSegment" />-instance to fill.</param>
        /// <exception cref="UnexpectedTokenException">Thrown when an unexpected token was found.</exception>
        internal static void ParseConstantRoute(ControllerRouteSegment segment)
        {
            RouteParser.Expect(RouteTokenType.NAME);

            segment.Type = ControllerRouteSegmentType.Constant;
            segment.Name = RouteParser.CurrentToken.Value;
            segment.Default = null;

            RouteParser.Skip();
            RouteParser.Expect(RouteTokenType.EOF);
        }

        /// <summary>
        /// Parse the current source into a variable <see cref="ControllerRouteSegment" />-instance.
        /// </summary>
        /// <param name="segment">The <see cref="ControllerRouteSegment" />-instance to fill.</param>
        /// <exception cref="UnexpectedTokenException">Thrown when an unexpected token was found.</exception>
        internal static void ParseVariableRoute(ControllerRouteSegment segment)
        {
            RouteParser.Expect(RouteTokenType.BRACE_LEFT);
            RouteParser.Skip();

            RouteParser.Expect(RouteTokenType.NAME);

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
            RouteParser.Expect(RouteTokenType.EOF);
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