using chess.v4.models.enumeration;
using chess.v4.engine.interfaces;
using common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using tests.setup;
using tests.utility;

namespace tests {

	[TestClass]
	public class DatabaseStorageTests {
		public IGameStateService GameStateService { get; }

		public DatabaseStorageTests() {
			var serviceProvider = new TestSetup().Setup();
			this.GameStateService = serviceProvider.GetService<IGameStateService>();
		}

		[TestMethod]
		public void InsertTest() {

		}
	}
}