using Dapper;
using Data.Dapper.Models;
using Data.Interfaces;
using Data.Models.Application;
using Data.Models.Entities;
using Data.Repository.FunctionDefinitions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Data.Repository.Dapper {
	public class ClientDapperBaseRepository : DapperAsyncRepository, IEntityDapperRepository<Client, long> {
		public ClientDapperBaseRepository(IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory) : base(appSettings, loggerFactory){ }

		public ClientDapperBaseRepository(string connectionString, IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory) : base(connectionString, appSettings, loggerFactory){ }

		public virtual async Task<InsertResponse<long>> Create(Client client) {
			var sql = "Execute [Membership].[Client_Insert] @Name, @Code, @Secret, @IsActive, @CreatedById, @CreatedDate, @UpdatedById, @UpdatedDate, @Id OUTPUT";
			var _params = new DynamicParameters();
			_params.Add("Name", client.Name);
			_params.Add("Code", client.Code);
			_params.Add("Secret", client.Secret);
			_params.Add("IsActive", client.IsActive);
			_params.Add("CreatedById", client.CreatedById);
			_params.Add("CreatedDate", client.CreatedDate);
			_params.Add("UpdatedById", client.UpdatedById);
			_params.Add("UpdatedDate", client.UpdatedDate);
	_params.Add("Id", dbType: DbType.Int64, direction: ParameterDirection.Output);
			var result = await base.ExecuteAsync(sql, _params);
			return InsertResponse<long>.GetInsertResponse(result);
		}

		public virtual async Task<TransactionResponse> Update(Client client) {
			var sql = "Execute [Membership].[Client_Update] @Id, @Name, @Code, @Secret, @IsActive, @UpdatedById, @UpdatedDate";
			var result = await base.ExecuteAsync(sql, client);
			return result;
		}

		public virtual async Task<TransactionResponse> Delete(long id) {
			var sql = "Execute [Membership].[Client_Delete] @id";
			var result = await base.ExecuteAsync(sql, new { 
				id = id,
			});
			return result;
		}

		public virtual async Task<Client> SelectById(long id) {
			return await this.QuerySingleAsync<Client>(new Client_SelectById_Function(id));
		}

		public virtual async Task<IDataResult<Client>> ReadAll(PageInfo pageInfo) {
			return await this.QueryAsync<Client>(new Client_ReadAll_Function(), pageInfo);
		}
	}
}