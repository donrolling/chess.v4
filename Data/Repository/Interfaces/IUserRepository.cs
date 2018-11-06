using Data.Interfaces;
using Models.Entities;
using System.Threading.Tasks;

namespace Data.Repository.Dapper.Interfaces {
	public interface IUserRepository : IEntityDapperRepository<User, long> {

		Task<User> Select_ByLogin(string login);
	}
}