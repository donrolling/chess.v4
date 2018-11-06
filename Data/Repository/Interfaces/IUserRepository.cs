using Data.Interfaces;
using Models.Application;
using Models.Entities;
using System;
using System.Threading.Tasks;

namespace Data.Repository.Interfaces {
	public interface IUserRepository : IEntityDapperRepository<User, long> {
		Task<User> Select_ByLogin(string login);
	}
}