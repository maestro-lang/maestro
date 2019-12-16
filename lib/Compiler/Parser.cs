namespace Rain
{
	internal sealed class Parser
	{
		public readonly Tokenizer tokenizer;
		private readonly System.Action<Slice, CompileErrorType, ICompileErrorContext> onError;

		public Token previousToken;
		public Token currentToken;

		public Parser(Tokenizer tokenizer, System.Action<Slice, CompileErrorType, ICompileErrorContext> onError)
		{
			this.tokenizer = tokenizer;
			this.onError = onError;

			Reset(new Token(TokenKind.End, new Slice()), new Token(TokenKind.End, new Slice()));
		}

		public void Reset(Token previousToken, Token currentToken)
		{
			this.previousToken = previousToken;
			this.currentToken = currentToken;
		}

		public void Next()
		{
			previousToken = currentToken;

			while (true)
			{
				currentToken = tokenizer.Next();
				if (currentToken.kind != TokenKind.Error)
					break;

				onError(currentToken.slice, CompileErrorType.InvalidToken, null);
			}
		}

		public bool Check(TokenKind tokenKind)
		{
			return currentToken.kind == tokenKind;
		}

		public bool Match(TokenKind tokenKind)
		{
			if (currentToken.kind != tokenKind)
				return false;

			Next();
			return true;
		}

		public void Consume(TokenKind tokenKind, CompileErrorType consumeError)
		{
			if (currentToken.kind == tokenKind)
				Next();
			else
				onError(currentToken.slice, consumeError, null);
		}

		public void Consume<C>(TokenKind tokenKind, CompileErrorType consumeError, C context) where C : struct, ICompileErrorContext
		{
			if (currentToken.kind == tokenKind)
				Next();
			else
				onError(currentToken.slice, consumeError, context);
		}
	}
}