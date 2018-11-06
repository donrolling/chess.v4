using System.Linq;
using System.Threading.Tasks;
using Data.Dapper.Models;
using Data.Repository.Dapper;
using Data.Repository.Dapper.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Application;
using Models.Entities;

namespace Data.Repository.Interfaces {
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