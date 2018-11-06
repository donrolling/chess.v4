using Business.Interfaces;
using chess.v4.engine.interfaces;
using chess.v4.engine.service;
using Common.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Application;
using NLog;
using System;

namespace Tests.Models {

	public class TestBase {
		public IOptions<AppSettings> AppSettings { get; }
		public Microsoft.Extensions.Logging.ILogger Logger { get; }
		public ILoggerFactory LoggerFactory { get; }
		public IMembershipService MembershipService { get; private set; }
		public IServiceProvider ServiceProvider { get; }
		public TestContext TestContext { get; set; }
		public string TestName {
			get {
				return this.TestContext.TestName;
			}
		}

		public TestBase() {
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

			this.AppSettings = this.ServiceProvider.GetService<IOptions<AppSettings>>();

			this.ServiceProvider = services.BuildServiceProvider();
		}
	}
}