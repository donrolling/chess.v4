using AutoFixture;
using Business.Service.EntityServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Entities;
using System.Threading.Tasks;
using Tests.Models;

namespace Tests.Tests.Entity {
	[TestClass]
	public class GameIntegrationTests : TestBase {
		public IGameService GameService { get; private set; }

		public GameIntegrationTests() {
			this.GameService = this.ServiceProvider.GetService<IGameService>();
		}

		[TestMethod]
		public async Task CRUD_Game_GivenValidValues_Succeeds() {

			var fixture = new Fixture();
			var game = fixture.Build<Game>()
				.Without(a => a.Id)
				.Without(a => a.CreatedDate)
				.Without(a => a.CreatedById)
				.Without(a => a.UpdatedDate)
				.Without(a => a.UpdatedById)
				.Create();
			Assert.IsNotNull(game);

			//create object
			var createResult = await this.GameService.Create(game);
			Assert.IsTrue(createResult.Success);

			try{
				//select object by id to ensure that it was saved to db
				var newGame = await this.GameService.SelectById(createResult.Id);
				Assert.IsNotNull(newGame);

				//update object to ensure that it can be modified and saved to db
				//DO STUFF HERE - SUCH AS:
				//newGame.Name = "Something Random";
			
				//update the item in the database
				var updateResult = await this.GameService.Update(newGame);
				Assert.IsTrue(updateResult.Success);

				//verify that the data in the newly updated object is not the same as it was previously.
				var postUpdatedGame = this.GameService.SelectById(createResult.Id);
				Assert.IsNotNull(postUpdatedGame);
				//DO STUFF HERE TO ASSERT THAT THE CHANGES WERE MADE - SUCH AS:
				//Assert.AreNotEqual(game.Name, newGame.Name);
				} finally {
				//delete the item in the database
				var deleteResult = await this.GameService.Delete(createResult.Id);
				Assert.IsTrue(deleteResult.Success);

				//verify that the item was deleted
				var deleteConfirmGame = this.GameService.SelectById(createResult.Id);
				Assert.IsNull(deleteConfirmGame.Result);
			}
					}
	}
}