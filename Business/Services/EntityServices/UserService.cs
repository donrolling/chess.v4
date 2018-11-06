using Business.Interfaces;
using Business.Service.EntityServices.Interfaces;
using Business.Services.EntityServices.BaseServices;
using Common.Extensions;
using Data.Repository.Dapper.Interfaces;
using Microsoft.Extensions.Logging;
using Models.Entities;
using System.Threading.Tasks;

namespace Business.Services.EntityServices {
	public class UserService : UserBaseService, IUserService {

		public UserService(IMembershipService membershipService, IUserRepository userRepository, ILoggerFactory loggerFactory) : base(membershipService, userRepository, loggerFactory) {
		}
		
		public async Task<User> Select_ByLogin(string login) {
			if (string.IsNullOrEmpty(login)) {
				this.Logger.LogInformation("Select_ByLogin() login was empty.");
				return null;
			}
			try {
				var user = await this.UserRepository.Select_ByLogin(login);
				return user;
			} catch (System.Exception ex) {
				this.Logger.LogError(ex, "Select_ByLogin");
				throw;
			}
		}
	}
}