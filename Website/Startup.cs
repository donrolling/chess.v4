using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Interfaces;
using Business.Service.EntityServices.Interfaces;
using Business.Services.EntityServices;
using Business.Services.Membership;
using chess.v4.engine.interfaces;
using chess.v4.engine.service;
using Common.Interfaces;
using Common.Services;
using Common.Web.Interfaces;
using Common.Web.Services;
using Data.Repository.Dapper;
using Data.Repository.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace Website {
	public class Startup {
		public Startup(IConfiguration configuration) {
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services) {
			services.Configure<CookiePolicyOptions>(options => {
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			//var pathToNLogConfig = FileUtility.GetFullPath_FromRelativePath<TestBase>("nlog.config");
			//var pathToAppSettingsConfig = FileUtility.GetFullPath_FromRelativePath<TestBase>("appsettings.json");
			var provider = new PhysicalFileProvider("");
			//LogManager.Configuration = new XmlLoggingConfiguration(pathToNLogConfig.filePath);
			//var config = new ConfigurationBuilder().AddJsonFile(pathToAppSettingsConfig.filePath).Build();
			//services.Configure<AppSettings>(config.GetSection("AppSettings"));
			//var loggerFactory = new LoggerFactory().AddNLog();
			//services.AddSingleton<ILoggerFactory>(loggerFactory);
			//services.AddSingleton<IFileProvider>(provider);

			//generated
			services.AddTransient<IAppCacheService, AppCacheService>();
			services.AddTransient<ISessionCacheService, SessionCacheService>();
			//services.AddTransient<IHttpContextAccessor, FakeHttpContextAccessor>();
			services.AddTransient<IAuthenticationPersistenceService, AuthenticationPersistenceService>();
			services.AddTransient<IMembershipService, MembershipService>();

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

			
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
			} else {
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseCookiePolicy();

			app.UseMvc(routes => {
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
