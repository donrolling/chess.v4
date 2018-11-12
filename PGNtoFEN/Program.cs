using System;

namespace PGNtoFEN {
	class Program {
		static void Main(string[] args) {
			Console.WriteLine("Paste PGN string here:");
			var pgn = Console.ReadLine();
			Console.WriteLine("Converting PGN to FEN...");
			var converter = new Converter();
			var result = converter.Convert(pgn);
			Console.WriteLine("Done!");
			Console.WriteLine(result);
			Console.WriteLine("Press any key to exit.");
			Console.ReadLine();
		}
	}
}
