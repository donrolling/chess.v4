using Common.Models;
using Data.Dapper.Models;
using Models.DTOs;
using Models.Entities;
using Models.Interfaces;
using System.Threading.Tasks;

namespace Business.Service.EntityServices.Interfaces {
	public interface IUserService {

		Task<InsertResponse<long>> Create(User user);

		Task<TransactionResponse> Delete(long id);

		Task<bool> DoesUserAlreadyExist(string email);

		Task<IDataResult<User>> ReadAll(PageInfo pageInfo);

		Task<Envelope<Account>> Register(AccountRegistration accountRegistration);

		Task<User> SelectById(long id);

		Task<TransactionResponse> Update(User user);
	}
}