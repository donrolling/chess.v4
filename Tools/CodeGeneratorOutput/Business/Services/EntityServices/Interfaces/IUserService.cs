using Data.Dapper.Models;
using Models.Entities;
using Models.Interfaces;
using System.Threading.Tasks;

namespace Business.Service.EntityServices.Interfaces {
	public interface IUserService {
		Task<InsertResponse<long>> Create(User user);
		Task<TransactionResponse> Update(User user);
		Task<TransactionResponse> Delete(long id);
		Task<User> SelectById(long id);
		Task<IDataResult<User>> ReadAll(PageInfo pageInfo);
	}
}