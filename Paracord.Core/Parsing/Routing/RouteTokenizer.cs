namespace Paracord.Core.Parsing.Routing
{
    public class RouteTokenizer
    {
        /// <summary>
        /// List of punctuation tokens.
        /// </summary>
        private static readonly char[] PUNCTUATION_TOKENS =
        {
            '=',
            '{',
            '}',
        };

        /// <summary>
        /// List of alphanumeric tokens.
        /// </summary>
        private static readonly char[] ALPHANUMERIC_TOKENS =
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };

        /// <summary>
        /// The source document.
        /// </summary>
        public string Source = string.Empty;

        /// <summary>
        /// The position into the document currently being processed.
        /// </summary>
        private int CurrentIndex = 0;

        /// <summary>
        /// Initialize a new tokenizer without a source attached.
        /// </summary>
        public RouteTokenizer()
        {
            this.CurrentIndex = 0;
        }

        /// <summary>
        /// Initialize a new tokenizer with a source attached.
        /// </summary>
        /// <param name="source">The source to tokenize.</param>
        public RouteTokenizer(string source)
        {
            this.SetSource(source);
        }

        /// <summary>
        /// Reset the tokenizer with the specified source. This will reset the current internal index.
        /// </summary>
        /// <param name="source">The source to tokenize.</param>
        public void SetSource(string source)
        {
            this.Source = source;
            this.CurrentIndex = 0;
        }

        /// <summary>
        /// Get the next token in the document.
        /// </summary>
        /// <returns>The next token parsed.</returns>
        public IEnumerable<RouteToken> GetTokens()
        {
            RouteToken token;
            while((token = this.GetNextToken()).Type != RouteTokenType.EOF)
            {
                yield return token;
            }
        }

        /// <summary>
        /// Get the next token in the document.
        /// </summary>
        /// <returns>The next token parsed.</returns>
        public RouteToken GetNextToken()
        {
            RouteToken token = this.ReadToken();

            this.CurrentIndex = token.End;
            return token;
        }

        /// <summary>
        /// Read the token at the current position and return it.
        /// </summary>
        /// <returns>The parsed token.</returns>
        private RouteToken ReadToken()
        {
            if(string.IsNullOrEmpty(this.Source))
            {
                return new RouteToken(RouteTokenType.EOF, 0, 0);
            }

            this.MoveToNextToken();

            if(this.CurrentIndex >= this.Source.Length)
            {
                return new RouteToken(RouteTokenType.EOF, this.CurrentIndex, this.CurrentIndex);
            }

            char firstChar = this.Source[this.CurrentIndex];

            if(firstChar < ' ')
            {
                throw new InvalidDataException($"Invalid token {(int) firstChar}");
            }

            if(PUNCTUATION_TOKENS.Contains(firstChar))
            {
                return firstChar switch
                {
                    '=' => new RouteToken(RouteTokenType.EQUAL, this.CurrentIndex, this.CurrentIndex + 1),
                    '{' => new RouteToken(RouteTokenType.BRACE_LEFT, this.CurrentIndex, this.CurrentIndex + 1),
                    '}' => new RouteToken(RouteTokenType.BRACE_RIGHT, this.CurrentIndex, this.CurrentIndex + 1),

                    _ => new RouteToken(RouteTokenType.UNKNOWN, this.CurrentIndex, this.CurrentIndex + 1),
                };
            }

            if(('a' <= firstChar && firstChar <= 'z') ||
              ('A' <= firstChar && firstChar <= 'Z') ||
              ('0' <= firstChar && firstChar <= '9'))
            {
                return this.ParseNameToken();
            }

            return new RouteToken(RouteTokenType.UNKNOWN, firstChar.ToString(), this.CurrentIndex, this.CurrentIndex + 1);
        }

        /// <summary>
        /// Parse a name token on the current position.
        /// </summary>
        /// <returns>A valid name token.</returns>
        internal RouteToken ParseNameToken()
        {
            string name = this.GetUntil(c =>
            {
                return ('a' <= c && c <= 'z') || ('A' <= c && c <= 'Z') || ('0' <= c && c <= '9');
            });

            return new RouteToken(RouteTokenType.NAME, name, this.CurrentIndex, this.CurrentIndex + name.Length);
        }

        /// <summary>
        /// Move the position to the start of the next token to parse.
        /// </summary>
        private void MoveToNextToken()
        {
            int pos = this.CurrentIndex;

            while(pos < this.Source.Length)
            {
                // This looks rather insane, because it might be.
                // If you have a better and/or cleaner version, make a pull request.
                //
                // If the current token is a punctuation token, then we should look for the next alpha-numeric.
                // However, if the current token is an alpha-numeric, then we should look for the punctuation token.
                bool moveToNextPunct = PUNCTUATION_TOKENS.Contains(this.Source[this.CurrentIndex]) && ALPHANUMERIC_TOKENS.Contains(this.Source[pos]);
                bool moveToNextAlnum = ALPHANUMERIC_TOKENS.Contains(this.Source[this.CurrentIndex]) && PUNCTUATION_TOKENS.Contains(this.Source[pos]);

                if(moveToNextPunct || moveToNextAlnum)
                {
                    pos++;
                }
                else
                {
                    this.CurrentIndex = pos;
                    return;
                }
            }

            this.CurrentIndex = pos;
        }

        /// <summary>
        /// Get string of following content, until the predicate fails or until source ends.
        /// </summary>
        /// <param name="predicate">Retrieve characters until this predicate fails.</param>
        /// <returns>The next content in the source.</returns>
        internal string GetUntil(Func<char, bool> predicate)
        {
            char c;
            int idx = this.CurrentIndex;

            do
            {
                idx++;

                if(idx < this.Source.Length)
                {
                    c = this.Source[idx];
                }
                else
                {
                    break;
                }
            }
            while(predicate(c));

            int length = idx - this.CurrentIndex;

            return this.Source.Substring(this.CurrentIndex, length);
        }
    }
}