using Common;
using Models.Entities;
using Data.Repository.Interfaces;
using Business.Service.EntityServices.Interfaces;
using Data.Interfaces;
using System;
using Business.Interfaces;
using Microsoft.Extensions.Logging;
using Data.Dapper.Models;
using Business.Services.EntityServices.BaseServices;
using System.Threading.Tasks;

namespace Business.Services.EntityServices {
	public class UserService : UserBaseService, IUserService {		
		public IEntityDapperRepository<User, long> EntityRepository { get { return this.UserRepository; } }

		public UserService(IMembershipService membershipService, IUserRepository userRepository, ILoggerFactory loggerFactory) : base(membershipService, userRepository, loggerFactory){ }

		public async Task<TransactionResponse> Save(User user) {
			if (user.Id == 0) {
				return await this.Create(user);
			} else {
				return await this.Update(user);
			}
		}
	}
}