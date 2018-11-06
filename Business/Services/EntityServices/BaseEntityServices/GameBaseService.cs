using Business.Interfaces;
using Business.Services.EntityServices.Base;
using Common.Transactions;
using Data.Dapper.Models;
using Data.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Models.Base;
using Models.Entities;
using Models.Interfaces;
using System.Threading.Tasks;

namespace Business.Services.EntityServices.BaseServices {
	public class GameBaseService : EntityServiceBase {
		public IMembershipService MembershipService { get; set; }
		public IGameRepository GameRepository { get; set; }

		private static readonly Auditing _auditing = new Auditing();

		public GameBaseService(IMembershipService membershipService, IGameRepository gameRepository, ILoggerFactory loggerFactory) : base(_auditing, loggerFactory){
			this.MembershipService = membershipService;
			this.GameRepository = gameRepository;
		}

		public virtual async Task<InsertResponse<long>> Create(Game game){
			var prepareForSaveResult = await base.PrepareForSave_Async<Game, long>(game, await this.MembershipService.CurrentUserId());
			if (!prepareForSaveResult.IsValid) {
				return InsertResponse<long>.GetInsertResponse(TransactionResponse.GetTransactionResponse(ActionType.Create, Status.Failure, StatusDetail.Invalid, prepareForSaveResult.ValidationMessage));
			}
			var result = await this.GameRepository.Create(game);
			game.Id = result.Id;
			return result;
		}

		public virtual async Task<TransactionResponse> Update(Game game){
			var prepareForSaveResult = await base.PrepareForSave_Async<Game, long>(game, await this.MembershipService.CurrentUserId());
			if (!prepareForSaveResult.IsValid) {
				return TransactionResponse.GetTransactionResponse(ActionType.Update, Status.Failure, StatusDetail.Invalid, prepareForSaveResult.ValidationMessage);
			}
			var result = await this.GameRepository.Update(game);
			return result;
		}

		public virtual async Task<TransactionResponse> Delete(long id){
			var result = await this.GameRepository.Delete(id);
			return result;
		}

		public virtual async Task<Game> SelectById(long id) {
			return await this.GameRepository.SelectById(id);
		}

		public virtual async Task<IDataResult<Game>> ReadAll(PageInfo pageInfo) {
			return await this.GameRepository.ReadAll(pageInfo);
		}
	}
}