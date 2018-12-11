using Models.Base;
using System.ComponentModel.DataAnnotations;

namespace Models.Entities {
	public class User : Entity<long> {
		[StringLength(150, ErrorMessage = "Email cannot be longer than 150 characters.")]
		[Display(Name = "Email")]
		public string Email { get; set; }
		[StringLength(150, ErrorMessage = "Password cannot be longer than 150 characters.")]
		[Display(Name = "Password")]
		public string Password { get; set; }
		[StringLength(150, ErrorMessage = "Salt cannot be longer than 150 characters.")]
		[Display(Name = "Salt")]
		public string Salt { get; set; }
	}

	public class User_Properties : Entity_Properties {
		public const string Email = "Email";
		public const string Id = "Id";
		public const string Password = "Password";
		public const string Salt = "Salt";
	}
}