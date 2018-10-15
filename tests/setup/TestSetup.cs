using chess.v4.engine.interfaces;
using chess.v4.engine.service;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace tests.setup {

	internal class TestSetup {

		internal IServiceProvider Setup() {
			var services = new ServiceCollection();
			services.AddTransient<IAttackService, AttackService>();
			services.AddTransient<ICoordinateService, CoordinateService>();
			services.AddTransient<IDiagonalService, DiagonalService>();
			services.AddTransient<IGameStateService, GameStateService>();
			services.AddTransient<IMoveService, MoveService>();
			services.AddTransient<INotationService, NotationService>();
			services.AddTransient<IOrthogonalService, OrthogonalService>();
			services.AddTransient<IPGNFileService, PGNFileService>();
			services.AddTransient<IPGNService, PGNService>();
			return services.BuildServiceProvider();
		}
	}
}