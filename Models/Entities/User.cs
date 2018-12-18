using Models.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Entities {
	public class User : Entity<long> {
		[Required]
		[StringLength(150, ErrorMessage = "Email cannot be longer than 150 characters.")]
		[Display(Name = "Email")]
		public string Email { get; set; }
		[Required]
		[Display(Name = "Guid")]
		public Guid Guid { get; set; }
		[Required]
		[StringLength(150, ErrorMessage = "Password cannot be longer than 150 characters.")]
		[Display(Name = "Password")]
		public string Password { get; set; }
		[Required]
		[StringLength(150, ErrorMessage = "Salt cannot be longer than 150 characters.")]
		[Display(Name = "Salt")]
		public string Salt { get; set; }
		[Required]
		[StringLength(150, ErrorMessage = "Verification cannot be longer than 150 characters.")]
		public string Verification { get; set; }
	}

	public class User_Properties : Entity_Properties {
		public const string Email = "Email";
		public const string Guid = "Guid";
		public const string Id = "Id";
		public const string Password = "Password";
		public const string Salt = "Salt";
		public const string Verification = "Verification";
	}
}