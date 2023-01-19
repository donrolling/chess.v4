using chess_bootstrapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace chess_engine_tests.Application
{
	public class TestBase
	{
		private static IConfiguration _configuration;
		public static IConfiguration Configuration
		{ get { return _configuration; } }

		private static IServiceProvider _serviceProvider;
		protected IServiceProvider ServiceProvider
		{ get { return _serviceProvider; } }

		protected ILogger Logger { get; }

		public TestBase()
		{
			if (_configuration == null)
			{
				_configuration = new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
					.AddEnvironmentVariables()
					.Build();
			}
			if (_serviceProvider == null)
			{
				// add some test world services
				var services = new ServiceCollection();
				_serviceProvider = Bootstrapper.ConfigureServices(services, Configuration);
			}

			Logger = _serviceProvider.GetService<ILoggerFactory>().CreateLogger(this.GetType());
		}
	}
}