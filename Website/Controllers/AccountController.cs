using Business.Service.EntityServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using Website.Models;

namespace Website.Controllers {
	public class AccountController : Controller {
		public IUserService UserService { get; set; }

		public AccountController(IUserService userService) {
			UserService = userService;
		}

		public IActionResult Register() {
			return View();
		}

		[HttpPost]
		public IActionResult Register(AccountRegistration accountRegistration) {
			this.UserService.Register(accountRegistration);
			return this.RedirectToPage("/Index");
		}
	}
}