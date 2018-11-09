using Data.Dapper.Models;
using Models.Entities;
using Models.Interfaces;
using System.Threading.Tasks;

namespace Business.Service.EntityServices.Interfaces {
	public interface IGameService {
		Task<InsertResponse<long>> Create(Game game);
		Task<TransactionResponse> Update(Game game);
		Task<TransactionResponse> Delete(long id);
		Task<Game> SelectById(long id);
		Task<IDataResult<Game>> ReadAll(PageInfo pageInfo);
	}
}