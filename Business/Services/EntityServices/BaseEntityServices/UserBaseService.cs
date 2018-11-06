using Business.Interfaces;
using Business.Services.EntityServices.Base;
using Common.Transactions;
using Data.Dapper.Models;
using Data.Repository.Dapper.Interfaces;
using Microsoft.Extensions.Logging;
using Models.Base;
using Models.Entities;
using Models.Interfaces;
using System.Threading.Tasks;

namespace Business.Services.EntityServices.BaseServices {
	public class UserBaseService : EntityServiceBase {
		public IMembershipService MembershipService { get; set; }
		public IUserRepository UserRepository { get; set; }

		private static readonly Auditing _auditing = new Auditing();

		public UserBaseService(IMembershipService membershipService, IUserRepository userRepository, ILoggerFactory loggerFactory) : base(_auditing, loggerFactory) {
			this.MembershipService = membershipService;
			this.UserRepository = userRepository;
		}

		public virtual async Task<InsertResponse<long>> Create(User user) {
			var prepareForSaveResult = await base.PrepareForSave_Async<User, long>(user, await this.MembershipService.CurrentUserId());
			if (!prepareForSaveResult.IsValid) {
				return InsertResponse<long>.GetInsertResponse(TransactionResponse.GetTransactionResponse(ActionType.Create, Status.Failure, StatusDetail.Invalid, prepareForSaveResult.ValidationMessage));
			}
			var result = await this.UserRepository.Create(user);
			user.Id = result.Id;
			return result;
		}

		public virtual async Task<TransactionResponse> Delete(long id) {
			var result = await this.UserRepository.Delete(id);
			return result;
		}

		public virtual async Task<IDataResult<User>> ReadAll(PageInfo pageInfo) {
			return await this.UserRepository.ReadAll(pageInfo);
		}

		public virtual async Task<User> SelectById(long id) {
			return await this.UserRepository.SelectById(id);
		}

		public virtual async Task<TransactionResponse> Update(User user) {
			var prepareForSaveResult = await base.PrepareForSave_Async<User, long>(user, await this.MembershipService.CurrentUserId());
			if (!prepareForSaveResult.IsValid) {
				return TransactionResponse.GetTransactionResponse(ActionType.Update, Status.Failure, StatusDetail.Invalid, prepareForSaveResult.ValidationMessage);
			}
			var result = await this.UserRepository.Update(user);
			return result;
		}
	}
}