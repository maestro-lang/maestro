namespace Maestro
{
	internal sealed class Tokenizer
	{
		public string source;
		public int nextIndex;

		private readonly Scanner[] scanners;

		public Tokenizer(Scanner[] scanners)
		{
			this.scanners = scanners;
			Reset(string.Empty);
		}

		public void Reset(string source)
		{
			this.source = source;
			this.nextIndex = 0;
		}

		public Token Next()
		{
			while (nextIndex < source.Length)
			{
				var tokenLength = 0;
				var tokenKind = TokenKind.Error;
				foreach (var scanner in scanners)
				{
					var length = scanner.Scan(source, nextIndex);
					if (tokenLength >= length)
						continue;

					tokenLength = length;
					tokenKind = scanner.tokenKind;
				}

				if (tokenKind == TokenKind.End)
				{
					nextIndex += tokenLength;
					continue;
				}

				if (tokenLength == 0)
					tokenLength = 1;

				var token = new Token(tokenKind, new Slice(nextIndex, tokenLength));
				nextIndex += tokenLength;
				return token;
			}

			return new Token(TokenKind.End, new Slice(source.Length, 0));
		}
	}
}