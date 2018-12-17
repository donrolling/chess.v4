using Business.Interfaces;
using Business.Services.EntityServices.Base;
using Common.Models;
using Common.Transactions;
using Data.Dapper.Models;
using Data.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Application;
using Models.Base;
using Models.DTOs;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Services.Membership {
	public class MembershipService : EntityServiceBase, IMembershipService {
		public IOptions<AppSettings> AppSettings { get; set; }
		public IAuthenticationPersistenceService AuthenticationPersistenceService { get; }
		public IFileProvider FileProvider { get; private set; }
		public IHttpContextAccessor HttpContextAccessor { get; }
		public IUserRepository UserRepository { get; private set; }
		private const string _returnUrlQSParam = "?returnUrl=";
		private static readonly Auditing _auditing = new Auditing();
		/// <summary>
		/// If you don't have one of these roles, you're outta here.
		/// </summary>
		private static readonly List<string> _requiredRoles = new List<string> {
			RoleEnum.Guest.ToString()
		};

		public MembershipService(
			IAuthenticationPersistenceService authenticationPersistenceService,
			IUserRepository userRepository,
			IFileProvider fileProvider,
			IHttpContextAccessor httpContextAccessor,
			IOptions<AppSettings> appSettings,
			ILoggerFactory loggerFactory
		) : base(_auditing, loggerFactory) {
			this.AuthenticationPersistenceService = authenticationPersistenceService;
			this.UserRepository = userRepository;
			this.FileProvider = fileProvider;
			this.HttpContextAccessor = httpContextAccessor;
			this.AppSettings = appSettings;
		}

		public UserContext Current() {
			return this.AuthenticationPersistenceService.RetrieveUser();
		}

		public long CurrentUserId() {
			var user = this.Current();
			return user.Id;
		}

		public UserContext GuestUser() {
			return new UserContext {
				Email = "Guest",
				IsAuthenticated = false
			};
		}

		public bool HasClaim(System.Security.Claims.Claim claim) {
			//todo: fill this in for your application
			return true;
		}

		public async Task<Envelope<Account>> Login(string username, string password) {
			if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) {
				return Envelope<Account>.Fail("Neither username nor password may be empty.");
			}
			var pageInfo = new PageInfo(1);
			pageInfo.AddFilter(new SearchFilter(User_Properties.Email, username));
			var userQueryResult = await this.UserRepository.ReadAll(pageInfo);
			if (userQueryResult.Total == 0 || !userQueryResult.Data.Any()) {
				return Envelope<Account>.Fail("User not found.");
			}
			var user = userQueryResult.Data.First();
			var passwordService = new PasswordService();
			var encryptedPassword = passwordService.Generate_EncryptedPassword_Given_Password_Salt(password, user.Salt);
			if (encryptedPassword.EncryptedPassword != user.Password) {
				return Envelope<Account>.Fail("Password doesn't match.");
			}
			return Envelope<Account>.Ok(new Account(user.Guid, user.Email));
		}

		public async Task<Envelope<Account>> Register(AccountRegistration accountRegistration) {
			var alreadyExists = await this.UserRepository.DoesUserAlreadyExist(accountRegistration.Email);
			if (alreadyExists) {
				return Envelope<Account>.Fail("User already exists.", Status.Aborted);
			}
			var passwordService = new PasswordService();
			var passwordResult = passwordService.Generate_EncryptedPassword_Salt_Given_Password(accountRegistration.Password);
			var user = new User {
				Guid = Guid.NewGuid(),
				Email = accountRegistration.Email,
				Password = passwordResult.EncryptedPassword,
				Salt = passwordResult.Salt,
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

		public async Task<bool> UserHasAccess(string claimValue) {
			//fill this in for your application
			return await Task.Run(() => { return true; });
		}
	}
}