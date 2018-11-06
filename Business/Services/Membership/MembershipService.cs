using Business.Interfaces;
using Business.Services.Cookies;
using Business.Services.EntityServices.Base;
using Data.Repository.Dapper.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Application;
using Models.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Services.Membership {
	public class MembershipService : EntityServiceBase, IMembershipService {
		public IOptions<AppSettings> AppSettings { get; set; }
		public IAuthenticationPersistenceService AuthenticationPersistenceService { get; }
		public ICookieManager CookieManager { get; set; }
		public IFileProvider FileProvider { get; private set; }
		public IHttpContextAccessor HttpContextAccessor { get; }
		public IUserRepository UserRepository { get; private set; }
		private const string _returnUrlQSParam = "?returnUrl=";
		private static readonly Auditing _auditing = new Auditing();
		/// <summary>
		/// If you don't have one of these roles, you're outta here.
		/// </summary>
		private static readonly List<string> _requiredRoles = new List<string> {
			RoleEnum.Guest.ToString()
		};

		public MembershipService(
			IAuthenticationPersistenceService authenticationPersistenceService,
			ICookieManager cookieManager,
			IUserRepository userRepository,
			IFileProvider fileProvider,
			IHttpContextAccessor httpContextAccessor,
			IOptions<AppSettings> appSettings,
			ILoggerFactory loggerFactory
		) : base(_auditing, loggerFactory) {
			this.AuthenticationPersistenceService = authenticationPersistenceService;
			this.CookieManager = cookieManager;
			this.UserRepository = userRepository;
			this.FileProvider = fileProvider;
			this.HttpContextAccessor = httpContextAccessor;
			this.AppSettings = appSettings;
		}

		public async Task<UserContext> Current() {
			return await this.AuthenticationPersistenceService.RetrieveUser();
		}

		public async Task<long> CurrentUserId() {
			var user = await this.Current();
			return user.Id;
		}

		public UserContext GuestUser() {
			return new UserContext {
				Login = "Guest",
				IsAuthenticated = false
			};
		}

		public bool HasClaim(System.Security.Claims.Claim claim) {
			//todo: fill this in for your application
			return true;
		}

		public async Task<bool> UserHasAccess(string claimValue) {
			//fill this in for your application
			return await Task.Run(() => { return true; });
		}
	}
}