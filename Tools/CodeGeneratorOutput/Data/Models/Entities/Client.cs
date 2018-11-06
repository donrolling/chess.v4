using Data.Models.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models.Entities {
	public class Client : Entity<long> {
			[Required]
		[StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
		[Display(Name = "Name")]
		public string Name { get; set; }
		[Required]
		[Display(Name = "Code")]
		public Guid Code { get; set; }		
		[Required]
		[StringLength(250, ErrorMessage = "Secret cannot be longer than 250 characters.")]
		[Display(Name = "Secret")]
		public string Secret { get; set; }
	}

	public class Client_Properties : Entity_Properties {
		public const string Id = "Id";
		public const string Name = "Name";
		public const string Code = "Code";
		public const string Secret = "Secret";
	}
}