using Business.Service.EntityServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Entities;
using System.Threading.Tasks;
using Tests.Models;

namespace Tests.Tests.Entity {
	[TestClass]
	public class UserIntegrationTests : TestBase {
		public IUserService UserService { get; private set; }

		public UserIntegrationTests() {
			this.UserService = this.ServiceProvider.GetService<IUserService>();
		}

		[TestMethod]
		public async Task CRUD_User_GivenValidValues_Succeeds() {

			var fixture = new Fixture();
			var user = fixture.Build<User>()
				.Without(a => a.Id)
				.Without(a => a.CreatedDate)
				.Without(a => a.CreatedById)
				.Without(a => a.UpdatedDate)
				.Without(a => a.UpdatedById)
				.Create();
			Assert.IsNotNull(user);

			//create object
			var createResult = await this.UserService.Create(user);
			Assert.IsTrue(createResult.Success);

			try{
				//select object by id to ensure that it was saved to db
				var newUser = await this.UserService.SelectById(createResult.Id);
				Assert.IsNotNull(newUser);

				//update object to ensure that it can be modified and saved to db
				//DO STUFF HERE - SUCH AS:
				//newUser.Name = "Something Random";
			
				//update the item in the database
				var updateResult = await this.UserService.Update(newUser);
				Assert.IsTrue(updateResult.Success);

				//verify that the data in the newly updated object is not the same as it was previously.
				var postUpdatedUser = this.UserService.SelectById(createResult.Id);
				Assert.IsNotNull(postUpdatedUser);
				//DO STUFF HERE TO ASSERT THAT THE CHANGES WERE MADE - SUCH AS:
				//Assert.AreNotEqual(user.Name, newUser.Name);
				} finally {
				//delete the item in the database
				var deleteResult = await this.UserService.Delete(createResult.Id);
				Assert.IsTrue(deleteResult.Success);

				//verify that the item was deleted
				var deleteConfirmUser = this.UserService.SelectById(createResult.Id);
				Assert.IsNull(deleteConfirmUser.Result);
			}
					}
	}
}