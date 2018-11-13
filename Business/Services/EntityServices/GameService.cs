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
using System.Text.RegularExpressions;

namespace Business.Services.EntityServices {
	public class GameService : GameBaseService, IGameService {		
		public IEntityDapperRepository<Game, long> EntityRepository { get { return this.GameRepository; } }

		public GameService(IMembershipService membershipService, IGameRepository gameRepository, ILoggerFactory loggerFactory) : base(membershipService, gameRepository, loggerFactory){ }

		public override Task<InsertResponse<long>> Create(Game game) {
			var regex = new Regex("[ ]{2,}", RegexOptions.None);
			game.PGN = regex.Replace(game.PGN, " ");
			return base.Create(game);
		}

		public override Task<TransactionResponse> Update(Game game) {
			var regex = new Regex("[ ]{2,}", RegexOptions.None);
			game.PGN = regex.Replace(game.PGN, " ");
			return base.Update(game);
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