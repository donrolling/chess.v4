using Business.Interfaces;
using Business.Service.EntityServices.Interfaces;
using Business.Services.EntityServices.BaseServices;
using Data.Dapper.Models;
using Data.Interfaces;
using Data.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Models.Entities;
using System.Threading.Tasks;

namespace Business.Services.EntityServices {
	public class GameService : GameBaseService, IGameService {
		public IEntityDapperRepository<Game, long> EntityRepository { get { return this.GameRepository; } }

		public GameService(IMembershipService membershipService, IGameRepository gameRepository, ILoggerFactory loggerFactory) : base(membershipService, gameRepository, loggerFactory) {
		}

		public async Task<TransactionResponse> Save(Game game) {
			if (game.Id == 0) {
				return await this.Create(game);
			} else {
				return await this.Update(game);
			}
		}
	}
}