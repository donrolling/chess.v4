using chess_engine.Engine.Interfaces;
using chess_engine.Engine.Service;
using chess_engine.Models;
using Common.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;

namespace chess_bootstrapper
{
	public class Bootstrapper
	{
		/// <summary>
		/// This is probably the method you would use from a website or console app.
		/// </summary>
		/// <param name="appSettings"></param>
		/// <returns></returns>
		public static IServiceProvider ConfigureServices(IConfiguration configuration)
		{
			return ConfigureServices(new ServiceCollection(), configuration);
		}

		/// <summary>
		/// This is probably the method you would use from a test project where you might have some additional test services.
		/// </summary>
		/// <param name="appSettings"></param>
		/// <param name="services"></param>
		/// <returns></returns>
		public static IServiceProvider ConfigureServices(IServiceCollection services, IConfiguration configuration)
		{
			// logging
			// has to be configured in the IConfigutation
			Log.Logger = new LoggerConfiguration()
				.ReadFrom
				.Configuration(configuration)
				.CreateLogger();
			services.AddLogging(configure =>
				configure.AddSerilog(dispose: true)
			);

			// singletons
			var appSettings = FileIOEngine.ReadJSONConfig<AppSettings>("appsettings.json", "AppSettings");
			services.AddSingleton<AppSettings>(appSettings);

			// services
			services.AddTransient<IAttackService, AttackService>();
			services.AddTransient<IGameStateService, GameStateService>();
			services.AddTransient<IMoveService, MoveService>();
			services.AddTransient<ICheckmateService, CheckmateService>();
			services.AddTransient<INotationService, NotationService>();
			services.AddTransient<IOrthogonalService, OrthogonalService>();
			services.AddTransient<IPGNFileService, PGNFileService>();
			services.AddTransient<IPGNService, PGNService>();

			return services.BuildServiceProvider();
		}
	}
}