using Common.Extensions;
using Common.Logging;
using Microsoft.Extensions.Logging;
using System;
using Tests.Setup;

namespace Tests.models {
	public class BaseTest {
		public ILogger Logger { get; }
		public ILoggerFactory LoggerFactory { get; }
		protected IServiceProvider ServiceProvider { get; }

		public BaseTest() {
			this.ServiceProvider = new TestSetup().Setup();
			this.LoggerFactory = this.ServiceProvider.GetService<ILoggerFactory>();
			this.Logger = LogUtility.GetLogger(this.LoggerFactory, this.GetType());
		}
	}
}