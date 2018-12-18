using Common.Models;
using Models.Application;
using Models.DTOs;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Business.Interfaces {

	public interface IMembershipService {

		UserContext Current();

		long CurrentUserId();

		UserContext GuestUser();

		bool HasClaim(Claim claim);

		Task<Envelope<Account>> Login(string username, string password);

		Task<Envelope<Account>> Register(AccountRegistration accountRegistration, string url);

		Task<bool> UserHasAccess(string claimValue);

		Task<MethodResult> Verify(string verification);
	}
}