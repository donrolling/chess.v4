using Business.Service.EntityServices.Interfaces;
using Data.Dapper.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.DTOs;
using Models.Entities;
using System.Linq;
using System.Threading.Tasks;
using Tests.Models;
using Website.Models;

namespace Tests.SystemTests {

	[TestClass]
	public class AccountTests : TestBase {
		public IUserService UserService { get; }

		public AccountTests() {
			this.UserService = this.ServiceProvider.GetService<IUserService>();
		}

		[TestMethod]
		public async Task RegisterNewUserSuccessfully_ThenLogin() {
			var newUser = new AccountRegistration {
				Email = "donrolling@gmail.com",
				Password = "Password1"
			};

			//delete user if already there
			var pageInfo = new PageInfo(1);
			pageInfo.AddFilter(new SearchFilter(User_Properties.Email, newUser.Email));
			var userQueryResult = await this.UserService.ReadAll(pageInfo);
			if (userQueryResult.Total > 0) {
				var user = userQueryResult.Data.First();
				var deleteResult = await this.UserService.Delete(user.Id);
				Assert.IsTrue(deleteResult.Success, deleteResult.Message);
			}

			//register user
			var baseurl = "https://localhost:44347";
			var url = $"{ baseurl }{ Paths.AccountVerification }";
			var registrationResult = await this.MembershipService.Register(newUser, url);
			Assert.IsTrue(registrationResult.Success, registrationResult.Message);

			//verify user
			var verificationResult = await this.MembershipService.Verify(registrationResult.Result.Verification);
			Assert.IsTrue(verificationResult.Success, verificationResult.Message);
			
			//login user
			var loginResult = await this.MembershipService.Login(newUser.Email, newUser.Password);
			Assert.IsTrue(loginResult.Success, loginResult.Message);
		}
	}
}