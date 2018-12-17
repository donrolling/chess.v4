using Business.Interfaces;
using Business.Service.EntityServices.Interfaces;
using Business.Services.EntityServices;
using Business.Services.Membership;
using chess.v4.engine.interfaces;
using chess.v4.engine.service;
using Common.Interfaces;
using Common.IO;
using Common.Logging;
using Common.Services;
using Common.Web.Interfaces;
using Common.Web.Services;
using Data.Repository.Dapper;
using Data.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Application;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using System;
using Tests.Utilities;

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
			//basic config
			var pathToNLogConfig = FileUtility.GetFullPath_FromRelativePath<TestBase>("nlog.config");
			var pathToAppSettingsConfig = FileUtility.GetFullPath_FromRelativePath<TestBase>("appsettings.json");
			var provider = new PhysicalFileProvider(pathToNLogConfig.path);
			LogManager.Configuration = new XmlLoggingConfiguration(pathToNLogConfig.filePath);
			var config = new ConfigurationBuilder().AddJsonFile(pathToAppSettingsConfig.filePath).Build();
			var services = new ServiceCollection();
			services.Configure<AppSettings>(config.GetSection("AppSettings"));
			var loggerFactory = new LoggerFactory().AddNLog();
			services.AddSingleton<ILoggerFactory>(loggerFactory);
			services.AddSingleton<IFileProvider>(provider);

			//generated
			services.AddTransient<IAppCacheService, AppCacheService>();
			services.AddTransient<ISessionCacheService, SessionCacheService>();
			services.AddTransient<IHttpContextAccessor, FakeHttpContextAccessor>();
			services.AddTransient<IAuthenticationPersistenceService, Test_AuthenticationPersistenceService>();
			services.AddTransient<IMembershipService, MembershipService>();
			//services.AddTransient<IAuthenticationPersistenceService, AuthenticationPersistenceService>();
			services.AddTransient<IAuthenticationPersistenceService, Test_AuthenticationPersistenceService>();
			
			//chess services
			services.AddTransient<IAttackService, AttackService>();
			services.AddTransient<IGameStateService, GameStateService>();
			services.AddTransient<IMoveService, MoveService>();
			services.AddTransient<ICheckmateService, CheckmateService>();
			services.AddTransient<INotationService, NotationService>();
			services.AddTransient<IOrthogonalService, OrthogonalService>();
			services.AddTransient<IPGNFileService, PGNFileService>();
			services.AddTransient<IPGNService, PGNService>();
			services.AddTransient<ILoggerFactory, LoggerFactory>();

			//generated
			services.AddTransient<IGameService, GameService>();
			services.AddTransient<IGameRepository, GameDapperRepository>();
			services.AddTransient<IUserService, UserService>();
			services.AddTransient<IUserRepository, UserDapperRepository>();

			this.ServiceProvider = services.BuildServiceProvider();

			//settings
			this.AppSettings = this.ServiceProvider.GetService<IOptions<AppSettings>>();
			this.MembershipService = this.ServiceProvider.GetService<IMembershipService>();
			this.LoggerFactory = loggerFactory;
			this.Logger = LogUtility.GetLogger(loggerFactory, this.GetType());
		}
	}
}