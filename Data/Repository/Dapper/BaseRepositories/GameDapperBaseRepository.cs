using Dapper;
using Data.Dapper.Models;
using Data.Interfaces;
using Data.Repository.FunctionDefinitions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Application;
using Models.Entities;
using Models.Interfaces;
using System.Data;
using System.Threading.Tasks;

namespace Data.Repository.Dapper {
	public class GameDapperBaseRepository : DapperAsyncRepository, IEntityDapperRepository<Game, long> {
		public GameDapperBaseRepository(IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory) : base(appSettings, loggerFactory){ }

		public GameDapperBaseRepository(string connectionString, IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory) : base(connectionString, appSettings, loggerFactory){ }

		public virtual async Task<InsertResponse<long>> Create(Game game) {
			var sql = "Execute [dbo].[Game_Insert] @IsFinished, @Event, @Site, @Date, @Round, @White, @Black, @Result, @ECO, @WhiteElo, @BlackElo, @NaturalKey, @FileName, @Annotator, @Source, @Remark, @PGN, @FEN, @IsActive, @CreatedById, @CreatedDate, @UpdatedById, @UpdatedDate, @Id OUTPUT";
			var _params = new DynamicParameters();
			_params.Add("IsFinished", game.IsFinished);
			_params.Add("Event", game.Event);
			_params.Add("Site", game.Site);
			_params.Add("Date", game.Date);
			_params.Add("Round", game.Round);
			_params.Add("White", game.White);
			_params.Add("Black", game.Black);
			_params.Add("Result", game.Result);
			_params.Add("ECO", game.ECO);
			_params.Add("WhiteElo", game.WhiteElo);
			_params.Add("BlackElo", game.BlackElo);
			_params.Add("NaturalKey", game.NaturalKey);
			_params.Add("FileName", game.FileName);
			_params.Add("Annotator", game.Annotator);
			_params.Add("Source", game.Source);
			_params.Add("Remark", game.Remark);
			_params.Add("PGN", game.PGN);
			_params.Add("FEN", game.FEN);
			_params.Add("IsActive", game.IsActive);
			_params.Add("CreatedById", game.CreatedById);
			_params.Add("CreatedDate", game.CreatedDate);
			_params.Add("UpdatedById", game.UpdatedById);
			_params.Add("UpdatedDate", game.UpdatedDate);
	_params.Add("Id", dbType: DbType.Int64, direction: ParameterDirection.Output);
			var result = await base.ExecuteAsync(sql, _params);
			return InsertResponse<long>.GetInsertResponse(result);
		}

		public virtual async Task<TransactionResponse> Update(Game game) {
			var sql = "Execute [dbo].[Game_Update] @Id, @IsFinished, @Event, @Site, @Date, @Round, @White, @Black, @Result, @ECO, @WhiteElo, @BlackElo, @NaturalKey, @FileName, @Annotator, @Source, @Remark, @PGN, @FEN, @IsActive, @UpdatedById, @UpdatedDate";
			var result = await base.ExecuteAsync(sql, game);
			return result;
		}

		public virtual async Task<TransactionResponse> Delete(long id) {
			var sql = "Execute [dbo].[Game_Delete] @id";
			var result = await base.ExecuteAsync(sql, new { 
				id = id,
			});
			return result;
		}

		public virtual async Task<Game> SelectById(long id) {
			return await this.QuerySingleAsync<Game>(new Game_SelectById_Function(id));
		}

		public virtual async Task<IDataResult<Game>> ReadAll(PageInfo pageInfo) {
			return await this.QueryAsync<Game>(new Game_ReadAll_Function(), pageInfo);
		}
	}
}