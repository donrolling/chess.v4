using Business.Interfaces;
using Business.Services.EntityServices.Base;
using Common.Transactions;
using Data.Dapper.Enums;
using Data.Dapper.Models;
using Data.Interfaces;
using Data.Models.Base;
using Data.Models.Entities;
using Data.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Business.Services.EntityServices.BaseServices {
	public class ClientBaseService : EntityServiceBase {
		public IMembershipService MembershipService { get; set; }
		public IClientRepository ClientRepository { get; set; }

		private static readonly Auditing _auditing = new Auditing();

		public ClientBaseService(IMembershipService membershipService, IClientRepository clientRepository, ILoggerFactory loggerFactory) : base(_auditing, loggerFactory){
			this.MembershipService = membershipService;
			this.ClientRepository = clientRepository;
		}

		public virtual async Task<InsertResponse<long>> Create(Client client){
			var prepareForSaveResult = await base.PrepareForSave_Async<Client, long>(client, await this.MembershipService.CurrentUserId());
			if (!prepareForSaveResult.IsValid) {
				return InsertResponse<long>.GetInsertResponse(TransactionResponse.GetTransactionResponse(ActionType.Create, Status.Failure, StatusDetail.Invalid, prepareForSaveResult.ValidationMessage));
			}
			var result = await this.ClientRepository.Create(client);
			client.Id = result.Id;
			return result;
		}

		public virtual async Task<TransactionResponse> Update(Client client){
			var prepareForSaveResult = await base.PrepareForSave_Async<Client, long>(client, await this.MembershipService.CurrentUserId());
			if (!prepareForSaveResult.IsValid) {
				return TransactionResponse.GetTransactionResponse(ActionType.Update, Status.Failure, StatusDetail.Invalid, prepareForSaveResult.ValidationMessage);
			}
			var result = await this.ClientRepository.Update(client);
			return result;
		}

		public virtual async Task<TransactionResponse> Delete(long id){
			var result = await this.ClientRepository.Delete(id);
			return result;
		}

		public virtual async Task<Client> SelectById(long id) {
			return await this.ClientRepository.SelectById(id);
		}

		public virtual async Task<IDataResult<Client>> ReadAll(PageInfo pageInfo) {
			return await this.ClientRepository.ReadAll(pageInfo);
		}
	}
}