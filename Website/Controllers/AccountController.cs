using Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using System.Threading.Tasks;
using Website.Models;

namespace Website.Controllers {

	public class AccountController : Controller {
		public IMembershipService MembershipService { get; set; }

		public AccountController(IMembershipService membershipService) {
			MembershipService = membershipService;
		}

		public IActionResult Login() {
			return View();
		}

		[HttpPost]
		public IActionResult Login(AccountRegistration accountRegistration) {
			var result = this.MembershipService.Login(accountRegistration.Email, accountRegistration.Password);
			return this.RedirectToPage("/Index");
		}

		public IActionResult Register() {
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(AccountRegistration accountRegistration) {
			var baseurl = this.HttpContext.Request.PathBase;
			var url = $"{ baseurl }{ Paths.AccountVerification }";
			//var baseurl = string.Format("{0}://{1}{2}", .Url.Scheme, request.Url.Authority, (new System.Web.Mvc.UrlHelper(request.RequestContext)).Content("~"));
			var reuslt = await this.MembershipService.Register(accountRegistration, url);
			return this.RedirectToPage("/Index");
		}

		public async Task<IActionResult> Verify(string verification) {
			var verificationResult = await this.MembershipService.Verify(verification);
			if (verificationResult.Failure) {
				return this.RedirectToAction("Error", "Home");
			}
			return this.RedirectToAction("Index", "Home");
		}
	}
}