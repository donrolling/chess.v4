using Business.Interfaces;
using Business.Services.Cookies;
using Common.BaseClasses;
using Data.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Application;
using Models.Entities;
using Omu.ValueInjecter;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Services.Membership {
	public class DummyAuthenticationPersistenceService : LoggingWorker, IAuthenticationPersistenceService {
		public IOptions<AppSettings> AppSettings { get; }
		public ICookieManager CookieManager { get; set; }
		public IUserRepository UserRepository { get; }
		public const string AUTH_SESSION_KEY = "AuthenticationPersistenceService";
		public const string AuthenticationPersistenceCookie = "AuthenticationPersistenceCookie";

		public DummyAuthenticationPersistenceService(
			IUserRepository userRepository,
			IOptions<AppSettings> appSettings,
			ICookieManager cookieManager,
			ILoggerFactory loggerFactory
		) : base(loggerFactory) {
			AppSettings = appSettings;
			UserRepository = userRepository;
			this.CookieManager = cookieManager;
		}

		public async Task<UserContext> RetrieveUser() {
			try {
				var cookie = this.CookieManager.Get<UserContext>(AuthenticationPersistenceCookie);
				if (cookie != null) {
					return cookie;
				}
				var userContext = await createOrRetrieveUserFromDatabase();
				this.CookieManager.Set(
					AuthenticationPersistenceCookie,
					userContext,
					new CookieOptions {
						Expires = DateTimeOffset.Now.AddMinutes(20),
					}
				);
				return userContext;
			} catch (Exception ex) {
				this.Logger.LogError(ex, "Cookie misread.");
				this.CookieManager.Remove(AuthenticationPersistenceCookie);
			}
			throw new Exception("Cookie misread.");
		}

		private async Task<UserContext> createOrRetrieveUserFromDatabase() {
			var user = await this.UserRepository.Select_ByLogin(this.AppSettings.Value.TestLogin);
			if (user == null) {
				var _user = new User {
					Id = 1,
					Login = this.AppSettings.Value.TestLogin,
					IsActive = true,
					CreatedById = 1,
					CreatedDate = DateTime.Parse("2018-10-26 20:57:55.290"),
					UpdatedById = 1,
					UpdatedDate = DateTime.Parse("2018-10-26 20:57:55.290")
				};
				var result = await this.UserRepository.Create(_user);
				if (result.Failure) {
					throw new Exception(result.Message);
				}
				user = await this.UserRepository.SelectById(_user.Id);
			}
			return getUserContext(user);
		}

		private UserContext getUserContext(User user) {
			var userContext = new UserContext();
			userContext.InjectFrom(user);
			userContext.Claims = new List<System.Security.Claims.Claim> {
				new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Login),
				new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.WindowsAccountName, userContext.Login)
			};
			userContext.IsAuthenticated = true;
			return userContext;
		}
	}
}