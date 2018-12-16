using System;

namespace Models.DTOs {
	public class Account {
		public string Email { get; set; }
		public Guid Id { get; set; }

		public Account() {

		}

		public Account(Guid id, string email) {
			Id = id;
			Email = email;
		}
	}
}