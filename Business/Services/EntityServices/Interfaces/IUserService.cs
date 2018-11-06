using Data.Dapper.Models;
using Data.Interfaces;
using Models.Application;
using Models.Entities;
using Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Service.EntityServices.Interfaces {
	public interface IUserService {
		Task<InsertResponse<long>> Create(User user);

		Task<TransactionResponse> Delete(long id);

		Task<IDataResult<User>> ReadAll(PageInfo pageInfo);

		Task<User> SelectById(long id);

		Task<TransactionResponse> Update(User user);

		Task<User> Select_ByLogin(string login);
	}
}