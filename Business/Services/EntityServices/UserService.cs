using Business.Interfaces;
using Business.Service.EntityServices.Interfaces;
using Business.Services.EntityServices.BaseServices;
using Business.Services.Membership;
using Common.Models;
using Common.Transactions;
using Data.Dapper.Models;
using Data.Interfaces;
using Data.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Models.DTOs;
using Models.Entities;
using System;
using System.Threading.Tasks;

namespace Business.Services.EntityServices {
	public class UserService : UserBaseService, IUserService {
		public IEntityDapperRepository<User, long> EntityRepository { get { return this.UserRepository; } }

		public UserService(IMembershipService membershipService, IUserRepository userRepository, ILoggerFactory loggerFactory) : base(membershipService, userRepository, loggerFactory) {
		}

		public async Task<bool> DoesUserAlreadyExist(string email) {
			return await this.UserRepository.DoesUserAlreadyExist(email);
}

		public async Task<TransactionResponse> Save(User user) {
			if (user.Id == 0) {
				return await this.Create(user);
			} else {
				return await this.Update(user);
			}
		}
	}
}