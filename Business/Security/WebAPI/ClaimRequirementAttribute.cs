using Business.Services.Membership;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Security.Claims;

namespace Business.Security.WebAPI {
	public class ClaimRequirementAttribute : Attribute, IFilterMetadata {
		public Claim Claim { get; }

		public ClaimRequirementAttribute(Business.Security.ClaimTypes claimType, NavigationSections claimValue) {
			this.Claim = new Claim(claimType.ToString(), claimValue.ToString());
		}

		public ClaimRequirementAttribute(string claimType, string claimValue) {
			this.Claim = new Claim(claimType, claimValue);
		}
	}
}