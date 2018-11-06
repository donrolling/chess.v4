using Data.Dapper.Models;
using Data.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Application;
using Models.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Repository.Dapper {
	public class UserDapperRepository : UserDapperBaseRepository, IUserRepository {

		public UserDapperRepository(IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory) : base(appSettings, loggerFactory) {
		}

		public async Task<User> Select_ByLogin(string login) {
			var pageInfo = new PageInfo();
			pageInfo.AddFilter(new SearchFilter(User_Properties.Login, login));
			var result = await this.ReadAll(pageInfo);
			return result.Data.FirstOrDefault();
		}
	}
}