namespace Maestro
{
	internal static class TokenScanners
	{
		public static readonly Scanner[] scanners = new Scanner[] {
			new ExactScanner(";").ForToken(TokenKind.SemiColon),
			new ExactScanner("|").ForToken(TokenKind.Pipe),
			new ExactScanner(",").ForToken(TokenKind.Comma),

			new ExactScanner("(").ForToken(TokenKind.OpenParenthesis),
			new ExactScanner(")").ForToken(TokenKind.CloseParenthesis),
			new ExactScanner("{").ForToken(TokenKind.OpenCurlyBrackets),
			new ExactScanner("}").ForToken(TokenKind.CloseCurlyBrackets),

			new RealNumberScanner().ForToken(TokenKind.FloatLiteral),
			new IntegerNumberScanner().ForToken(TokenKind.IntLiteral),
			new StringScanner('"').ForToken(TokenKind.StringLiteral),
			new ExactScanner("false").ForToken(TokenKind.False),
			new ExactScanner("true").ForToken(TokenKind.True),

			new ExactScanner("if").ForToken(TokenKind.If),
			new ExactScanner("else").ForToken(TokenKind.Else),
			new ExactScanner("foreach").ForToken(TokenKind.ForEach),
			new ExactScanner("in").ForToken(TokenKind.In),
			new ExactScanner("export").ForToken(TokenKind.Export),
			new ExactScanner("command").ForToken(TokenKind.Command),
			new ExactScanner("return").ForToken(TokenKind.Return),

			new IdentifierScanner("", "_+-*/<>=").ForToken(TokenKind.Identifier),
			new IdentifierScanner("$", "_-").ForToken(TokenKind.Variable),
			new ExactScanner("$$").ForToken(TokenKind.InputVariable),

			new WhiteSpaceScanner().Ignore(),
			new LineCommentScanner("#").Ignore(),
		};
	}
}