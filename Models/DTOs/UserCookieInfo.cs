using System;

namespace Models.DTOs {
	public class UserCookieInfo {
		public string OriginalLogin { get; set; }
		public Guid UserSessionId { get; set; }
	}
}