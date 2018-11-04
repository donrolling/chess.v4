using chess.v4.engine.interfaces;
using chess.v4.engine.service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using System;

namespace tests.setup {

	internal class TestSetup {
		public const string ConnectionString = @"Data Source=971JT039H2\DROLLING;Initial Catalog=Chess;Integrated Security=SSPI;";

		internal IServiceProvider Setup() {
			var services = new ServiceCollection();
			services.AddTransient<IAttackService, AttackService>();
			services.AddTransient<IDiagonalService, DiagonalService>();
			services.AddTransient<IGameStateService, GameStateService>();
			services.AddTransient<IMoveService, MoveService>();
			services.AddTransient<INotationService, NotationService>();
			services.AddTransient<IOrthogonalService, OrthogonalService>();
			services.AddTransient<IPGNFileService, PGNFileService>();
			services.AddTransient<IPGNService, PGNService>();
			services.AddTransient<ILoggerFactory, LoggerFactory>();
			return services.BuildServiceProvider();
		}
	}
}