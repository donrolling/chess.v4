using Business.Service.EntityServices.Interfaces;
using Common.IO;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tests.Models;

namespace PGNImport {
	/// <summary>
	/// Going for quick and dirty here. Inheriting from a test class. Classy.
	/// </summary>
	public class Importer : TestBase {
		public IGameService GameService { get; }

		public Importer() {
			this.GameService = this.ServiceProvider.GetService<IGameService>();
		}

		public void Import() {
			var data = FileUtility.ReadTextFile<Importer>("December1.pgn", "Data\\Games");
			Console.WriteLine(data);
		}
	}
}