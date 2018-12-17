using Models.Application;
using System;
using System.Threading.Tasks;

namespace Business.Interfaces {
	public interface IAuthenticationPersistenceService {

		UserContext RetrieveUser();

		Task PersistUser(UserContext user, DateTime issueDate, DateTime expireDate, bool isPersistent = false);
	}
}