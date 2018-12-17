using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.DTOs;
using System.Threading.Tasks;
using Tests.Models;

namespace Tests {
	[TestClass]
	public class AccountTests : TestBase {

		public AccountTests() {
		}

		[TestMethod]
		public async Task RegisterNewUserSuccessfully_ThenLogin() {
			var newUser = new AccountRegistration {
				Email = "don.rolling@pcg.com",
				Password = "Password1"
			};
			var registrationResult = await this.MembershipService.Register(newUser);
			Assert.IsTrue(registrationResult.Success, registrationResult.Message);
			var loginResult = await this.MembershipService.Login(newUser.Email, newUser.Password);
			Assert.IsTrue(loginResult.Success, loginResult.Message);
		}
	}
}