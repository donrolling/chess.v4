﻿using System;

namespace PGNImport {
	class Program {
		static void Main(string[] args) {
			Console.WriteLine("Importing PGN Files Now!");
			var i = new Importer();
			var result = i.Import().Result;
			Console.WriteLine("Done!");
			Console.ReadLine();
		}
	}
}
