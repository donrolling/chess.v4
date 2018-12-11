using Data.Repository.FunctionDefinitions;
using Data.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Application;
using System.Threading.Tasks;

namespace Data.Repository.Dapper {
	public class UserDapperRepository : UserDapperBaseRepository, IUserRepository {

		public UserDapperRepository(IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory) : base(appSettings, loggerFactory) {
		}

		public async Task<bool> DoesUserAlreadyExist(string email) {
			return await this.QuerySingleAsync<bool>(new DoesUserExist_Function(email));
		}
	}
}