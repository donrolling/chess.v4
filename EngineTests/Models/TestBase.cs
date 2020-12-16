using Chess.v4.Engine.Interfaces;
using Chess.v4.Engine.Service;
using Common.IO;
using Common.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using System;

namespace Tests.Models
{
    public class TestBase
    {
        public Microsoft.Extensions.Logging.ILogger Logger { get; }
        public ILoggerFactory LoggerFactory { get; }
        public IServiceProvider ServiceProvider { get; }
        public TestContext TestContext { get; set; }

        public string TestName
        {
            get
            {
                return this.TestContext.TestName;
            }
        }

        public TestBase()
        {
            //basic config
            var pathToNLogConfig = FileUtility.GetFullPath_FromRelativePath<TestBase>("nlog.config");
            var provider = new PhysicalFileProvider(pathToNLogConfig.path);
            LogManager.Configuration = new XmlLoggingConfiguration(pathToNLogConfig.filePath);
            var services = new ServiceCollection();
            var loggerFactory = new LoggerFactory().AddNLog();
            services.AddSingleton<ILoggerFactory>(loggerFactory);
            services.AddSingleton<IFileProvider>(provider);

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

            this.ServiceProvider = services.BuildServiceProvider();

            //settings
            this.LoggerFactory = loggerFactory;
            this.Logger = LogUtility.GetLogger(loggerFactory, this.GetType());
        }
    }
}