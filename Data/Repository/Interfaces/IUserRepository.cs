using Data.Interfaces;
using Models.Entities;
using System.Threading.Tasks;

namespace Data.Repository.Interfaces {
	public interface IUserRepository : IEntityDapperRepository<User, long> {

		Task<bool> DoesUserAlreadyExist(string email);

		Task<User> Select_ByLogin(string email);
	}
}