using Business.Interfaces;
using Common.BaseClasses;
using Data.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Application;
using Models.Entities;
using Omu.ValueInjecter;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests.Utilities {
	public class Test_AuthenticationPersistenceService : LoggingWorker, IAuthenticationPersistenceService {
		public UserContext User { get; private set; }
		public IUserRepository UserRepository { get; }

		public Test_AuthenticationPersistenceService(IUserRepository userRepository, IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory) : base(loggerFactory) {
			UserRepository = userRepository;
			this.User = this.setupUser(appSettings.Value.TestLogin).Result;
		}

		public Task<UserContext> RetrieveUser() {
			return Task.Run(() => { return this.User; });
		}

		public async Task<bool> SignOut() {
			await Task.Run(() => {
				this.User = null;
			});
			return true;
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

		private async Task<UserContext> setupUser(string windowsLogin) {
			var user = await this.UserRepository.Select_ByLogin(windowsLogin);
			if (user == null) {
				var _user = new User { Login = windowsLogin, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow };
				var result = await this.UserRepository.Create(_user);
				if (result.Failure) {
					throw new Exception(result.Message);
				}
				user = await this.UserRepository.SelectById(_user.Id);
			}
			return getUserContext(user);
		}
	}
}