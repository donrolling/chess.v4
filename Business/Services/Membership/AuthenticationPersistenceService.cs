using Business.Interfaces;
using Business.Services.Cookies;
using Common.BaseClasses;
using Common.Web.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Models.Application;
using Models.DTOs;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Business.Services.Membership {
	public class AuthenticationPersistenceService : LoggingWorker, IAuthenticationPersistenceService {
		public ICookieManager CookieManager { get; set; }

		public IHttpContextAccessor HttpContextAccessor { get; }

		public ISessionCacheService SessionCacheService { get; }

		public const string AUTH_SESSION_KEY = "AuthenticationPersistenceService";

		public const string COOKIE_NAME = "AuthenticationPersistenceCookie";

		public const string COOKIE_NAME2 = ".AspNetCore.AuthCookie";

		public AuthenticationPersistenceService(
			ISessionCacheService sessionCacheService,
			IHttpContextAccessor httpContextAccessor,
			ICookieManager cookieManager,
			ILoggerFactory loggerFactory
		) : base(loggerFactory) {
			this.HttpContextAccessor = httpContextAccessor;
			this.SessionCacheService = sessionCacheService;
			this.CookieManager = cookieManager;
		}

		public async Task PersistUser(UserContext user, DateTime issueDate, DateTime expireDate, bool isPersistent = false) {
			var identity = new ClaimsIdentity(user.Claims, AuthenticationSchemes.AuthCookie);
			var userPrincipal = new ClaimsPrincipal(identity);
			var authenticationProperties = new AuthenticationProperties {
				ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
				IsPersistent = true,
				AllowRefresh = true
			};
			try {
				await this.HttpContextAccessor.HttpContext.SignInAsync(AuthenticationSchemes.AuthCookie, userPrincipal, authenticationProperties);
			} catch (Exception ex) {
				this.Logger.LogError(ex, "Sign In Error");
				throw;
			}
			var userInfo = new UserCookieInfo {
				UserSessionId = Guid.NewGuid(),
				OriginalLogin = user.Email
			};
			this.CookieManager.Set(COOKIE_NAME, userInfo,
				new CookieOptions {
					Expires = DateTimeOffset.Now.AddMinutes(20),
				}
			);
			this.SessionCacheService.Set(this.getKey(userInfo.UserSessionId), user, true);
		}

		public UserContext RetrieveUser() {
			var userInfo = this.CookieManager.Get<UserCookieInfo>(COOKIE_NAME);
			if (userInfo.UserSessionId == Guid.Empty) {
				return null;
			}
			return null;
			//return this.SessionCacheService.Get<UserContext>(this.getKey(userInfo.UserSessionId), true);
		}

		private string getKey(Guid userSessionId) {
			return $"{ AUTH_SESSION_KEY }-{ userSessionId.ToString() }";
		}
	}

	public class AuthenticationSchemes {
		public const string AuthCookie = "AuthCookie";
	}
}