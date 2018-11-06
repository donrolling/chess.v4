using Business.Models;
using Common.Interfaces;
using Models.Application;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Business.Interfaces {
	public interface IMembershipService {
		IAppCacheService AppCacheManager { get; }

		Task<UserContext> Current();

		Task<long> CurrentUserId();

		UserContext GuestUser();

		bool HasClaim(Claim claim);

		Task<bool> UserHasAccess(string claimValue);
	}
}