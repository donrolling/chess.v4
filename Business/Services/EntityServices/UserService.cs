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

		public async Task<Envelope<Account>> Register(AccountRegistration accountRegistration) {
			var alreadyExists = await this.DoesUserAlreadyExist(accountRegistration.Email);
			if (alreadyExists) {
				return Envelope<Account>.Fail("User already exists.", Status.Aborted);
			}
			var passwordService = new PasswordService();
			var psResult = passwordService.Generate_EncryptedPassword_Salt_Given_Password(accountRegistration.Password);
			var user = new User {
				Email = accountRegistration.Email,
				Password = psResult.EncryptedPassword,
				Salt = psResult.Salt,
				//IsActive = false //set isactive false until they register their email
			};
			var result = await this.UserRepository.Create(user);
			if (result.Failure) {
				return Envelope<Account>.Fail(result.Message);
			}
			var account = new Account { Email = user.Email };
			//make them verify their email and stuff
			return Envelope<Account>.Ok(account);
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