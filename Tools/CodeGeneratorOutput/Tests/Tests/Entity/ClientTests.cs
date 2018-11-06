using AutoFixture;
using Business.Interfaces;
using Business.Service.EntityServices.Interfaces;
using Data.Models.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Tests.Models;
using Tests.Utilities;

namespace Tests.Tests.Entity {
	[TestClass]
	public class ClientIntegrationTests : TestBase {
		public IMembershipService MembershipService { get; private set; }
		public IClientService ClientService { get; private set; }

		public ClientIntegrationTests() {
			this.MembershipService = MembershipHelper.GetMembershipService_And_SetCurrentUser(this.TestEnv, this.AppSettings).Result;
			this.ClientService = this.TestEnv.ServiceProvider.GetService<IClientService>();
		}

		[TestMethod]
		public async Task CRUD_Client_GivenValidValues_Succeeds() {

			var fixture = new Fixture();
			var client = fixture.Build<Client>()
				.Without(a => a.Id)
				.Without(a => a.CreatedDate)
				.Without(a => a.CreatedById)
				.Without(a => a.UpdatedDate)
				.Without(a => a.UpdatedById)
				.Create();
			Assert.IsNotNull(client);

			//create object
			var createResult = await this.ClientService.Create(client);
			Assert.IsTrue(createResult.Success);

			try{
				//select object by id to ensure that it was saved to db
				var newClient = await this.ClientService.SelectById(createResult.Id);
				Assert.IsNotNull(newClient);

				//update object to ensure that it can be modified and saved to db
				//DO STUFF HERE - SUCH AS:
				//newClient.Name = "Something Random";
			
				//update the item in the database
				var updateResult = await this.ClientService.Update(newClient);
				Assert.IsTrue(updateResult.Success);

				//verify that the data in the newly updated object is not the same as it was previously.
				var postUpdatedClient = this.ClientService.SelectById(createResult.Id);
				Assert.IsNotNull(postUpdatedClient);
				//DO STUFF HERE TO ASSERT THAT THE CHANGES WERE MADE - SUCH AS:
				//Assert.AreNotEqual(client.Name, newClient.Name);
				} finally {
				//delete the item in the database
				var deleteResult = await this.ClientService.Delete(createResult.Id);
				Assert.IsTrue(deleteResult.Success);

				//verify that the item was deleted
				var deleteConfirmClient = this.ClientService.SelectById(createResult.Id);
				Assert.IsNull(deleteConfirmClient.Result);
			}
					}
	}
}