using System;

namespace PGNtoFEN {
	class Program {
		static void Main(string[] args) {
			Console.WriteLine("Convert PGN to FEN!");
			repl();
			Console.WriteLine("Goodnight.");
		}

		private static void repl() {
			Console.WriteLine("Paste PGN string here or type exit:");
			var pgn = Console.ReadLine();
			if (pgn == "exit") {
				return;
			}
			var converter = new Converter();
			var result = converter.Convert(pgn);
			Console.WriteLine(result);
			repl();
		}
	}
}
