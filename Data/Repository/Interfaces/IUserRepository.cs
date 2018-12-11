using Data.Interfaces;
using Models.Entities;
using System;
using System.Threading.Tasks;

namespace Data.Repository.Interfaces {
	public interface IUserRepository : IEntityDapperRepository<User, long> {
		Task<bool> DoesUserAlreadyExist(string email);
	}
}