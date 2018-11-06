using Models.Application;
using System.Threading.Tasks;

namespace Business.Interfaces {
	public interface IAuthenticationPersistenceService {

		Task<UserContext> RetrieveUser();
	}
}