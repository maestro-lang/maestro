﻿using System.Text;

namespace Flow
{
	public static class Program
	{
		public sealed class PrintCommand : ICommand<Tuple0>
		{
			public void Execute(ref Context context, Tuple0 args)
			{
				var sb = new StringBuilder();
				for (var i = 0; i < context.inputCount; i++)
				{
					sb.Clear();
					context.GetInput(i).AppendTo(sb);
					sb.Append(' ');
					System.Console.Write(sb.ToString());
				}
			}
		}

		public sealed class BypassCommand : ICommand<Tuple0>
		{
			public void Execute(ref Context context, Tuple0 args)
			{
				for (var i = 0; i < context.inputCount; i++)
					context.PushValue(context.GetInput(i));
			}
		}

		public sealed class ElementsCommand : ICommand<Tuple0>
		{
			public int currentIndex = 0;

			public void Execute(ref Context context, Tuple0 args)
			{
				if (currentIndex < context.inputCount)
					context.PushValue(context.GetInput(currentIndex));
				else
					currentIndex = 0;
			}
		}

		public static void Main(string[] args)
		{
			var content = System.IO.File.ReadAllText("scripts/script.flow");
			var source = new Source(new Uri("script.flow"), content);

			var engine = new Engine();
			engine.RegisterCommand("print", () => new PrintCommand());
			engine.RegisterCommand("bypass", () => new BypassCommand());
			engine.RegisterCommand("elements", () => new ElementsCommand());

			var sb = new StringBuilder();

			var compileResult = engine.CompileSource(source, Mode.Debug, null);
			if (compileResult.HasErrors)
			{
				sb.Clear();
				compileResult.FormatErrors(sb);
				System.Console.WriteLine(sb);
			}
			else
			{
				sb.Clear();
				compileResult.FormatDisassembledByteCode(sb);
				System.Console.WriteLine(sb);

				var executeResult = engine.Execute(compileResult);
				if (executeResult.HasError)
				{
					sb.Clear();
					executeResult.FormatError(sb);
					executeResult.FomratCallStackTrace(sb);
					System.Console.WriteLine(sb);
				}

				System.Console.WriteLine("FINISH");
			}
		}
	}
}
