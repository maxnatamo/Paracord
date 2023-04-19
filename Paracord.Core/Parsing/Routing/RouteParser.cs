using Paracord.Core.Controller;
using Paracord.Shared.Exceptions;

namespace Paracord.Core.Parsing.Routing
{
    public partial class RouteParser
    {
        /// <summary>
        /// The <see cref="RouteTokenizer" />-instance to transform inputs into tokens.
        /// </summary>
        internal RouteTokenizer Tokenizer = new RouteTokenizer();

        /// <summary>
        /// The current position into the token-list
        /// </summary>
        internal int CurrentTokenIndex { get; set; } = 0;

        /// <summary>
        /// The currently-processing token
        /// </summary>
        internal RouteToken CurrentToken = new RouteToken();

        /// <summary>
        /// Parse the specified route into a list of <see cref="ControllerRouteSegment" />-instances.
        /// </summary>
        /// <param name="route">The route to parse.</param>
        /// <returns>List of <see cref="ControllerRouteSegment" />-instances.</returns>
        /// <exception cref="MissingBraceException">Thrown when the number of braces isn't even.</exception>
        /// <exception cref="UnexpectedTokenException">Thrown when an unexpected token was found.</exception>
        public List<ControllerRouteSegment> Parse(string route)
        {
            route = route.Trim('/');

            if(string.IsNullOrEmpty(route))
            {
                return new List<ControllerRouteSegment>();
            }

            this.Tokenizer.SetSource(route);
            this.CurrentTokenIndex = 0;
            this.CurrentToken = this.Tokenizer.GetNextToken();

            List<ControllerRouteSegment> segments = new List<ControllerRouteSegment>();

            segments.Add(this.ParseDefinition());

            while(this.Peek(RouteTokenType.SLASH))
            {
                this.Skip();
                segments.Add(this.ParseDefinition());
            }

            return segments;
        }

        /// <summary>
        /// Parse the current source into a <see cref="ControllerRouteSegment" />-instance.
        /// </summary>
        /// <returns>A non-null <see cref="ControllerRouteSegment" />, if a definition was found. Otherwise, null.</returns>
        /// <exception cref="UnexpectedTokenException">Thrown when an unexpected token was found.</exception>
        internal ControllerRouteSegment ParseDefinition()
        {
            if(this.Peek(RouteTokenType.NAME))
            {
                return this.ParseConstantRoute();
            }

            if(this.Peek(RouteTokenType.BRACE_LEFT))
            {
                return this.ParseVariableRoute();
            }

            throw UnexpectedToken();
        }

        /// <summary>
        /// Parse the current source into a constant <see cref="ControllerRouteSegment" />-instance.
        /// </summary>
        /// <returns>The parsed <see cref="ControllerRouteSegment" />-object.</returns>
        /// <exception cref="UnexpectedTokenException">Thrown when an unexpected token was found.</exception>
        internal ControllerRouteSegment ParseConstantRoute()
        {
            this.Expect(RouteTokenType.NAME);

            ControllerRouteSegment segment = new ControllerRouteSegment();
            segment.Type = ControllerRouteSegmentType.Constant;
            segment.Name = this.CurrentToken.Value;
            segment.Default = null;

            this.Skip();
            this.Expect(RouteTokenType.EOF, RouteTokenType.SLASH);

            return segment;
        }

        /// <summary>
        /// Parse the current source into a variable <see cref="ControllerRouteSegment" />-instance.
        /// </summary>
        /// <returns>The parsed <see cref="ControllerRouteSegment" />-object.</returns>
        /// <exception cref="UnexpectedTokenException">Thrown when an unexpected token was found.</exception>
        internal ControllerRouteSegment ParseVariableRoute()
        {
            this.Expect(RouteTokenType.BRACE_LEFT);
            this.Skip();

            this.Expect(RouteTokenType.NAME);

            ControllerRouteSegment segment = new ControllerRouteSegment();
            segment.Type = ControllerRouteSegmentType.Variable;
            segment.Name = this.CurrentToken.Value;
            this.Skip();

            if(segment.Name.Any(v => v >= '0' && v <= '9'))
            {
                throw UnexpectedToken();
            }

            if(this.Peek(RouteTokenType.COLON))
            {
                this.Skip();
                segment.ConstraintName = this.CurrentToken.Value;

                this.Skip();
            }

            if(this.Peek(RouteTokenType.EQUAL))
            {
                this.Skip();
                segment.Default = this.CurrentToken.Value;

                this.Skip();
            }

            this.Expect(RouteTokenType.BRACE_RIGHT);
            this.Skip();

            this.Expect(RouteTokenType.EOF, RouteTokenType.SLASH);

            return segment;
        }

        /// <summary>
        /// Peek the type of the current token.
        /// </summary>
        /// <param name="type">The token-type to peek for.</param>
        /// <returns>True, if the type matches the current token. Otherwise, false.</returns>
        internal bool Peek(RouteTokenType type)
        {
            return this.CurrentToken.Type == type;
        }

        /// <summary>
        /// Peek the type of the current token and throw if it doesn't match any of the specified types.
        /// </summary>
        /// <param name="types">The token-types to assert for.</param>
        /// <exception cref="UnexpectedTokenException">Thrown if the assertion fails.</exception>
        internal void Expect(params RouteTokenType[] types)
        {
            if(!types.Any(v => this.Peek(v)))
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
        internal void Skip()
        {
            this.CurrentTokenIndex++;
            this.CurrentToken = this.Tokenizer.GetNextToken();
        }

        /// <summary>
        /// Return <see cref="UnexpectedTokenException" />-object with location attached as description.
        /// </summary>
        /// <returns><see cref="UnexpectedTokenException" />-object.</returns>
        internal Exception UnexpectedToken()
            => new UnexpectedTokenException(this.CurrentToken.Start);
    }
}