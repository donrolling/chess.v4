using Business.Interfaces;
using Business.Service.EntityServices.Interfaces;
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

		public IActionResult Register() {
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(AccountRegistration accountRegistration) {
			var reuslt = await this.MembershipService.Register(accountRegistration);
			return this.RedirectToPage("/Index");
		}

		public IActionResult Login() {
			return View();
		}

		[HttpPost]
		public IActionResult Login(AccountRegistration accountRegistration) {
			var result = this.MembershipService.Login(accountRegistration.Email, accountRegistration.Password);
			return this.RedirectToPage("/Index");
		}
	}
}