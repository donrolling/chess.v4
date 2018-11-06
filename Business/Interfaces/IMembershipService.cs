using Models.Application;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Business.Interfaces {
	public interface IMembershipService {

		Task<UserContext> Current();

		Task<long> CurrentUserId();

		UserContext GuestUser();

		bool HasClaim(Claim claim);

		Task<bool> UserHasAccess(string claimValue);
	}
}