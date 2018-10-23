namespace common {

	public class Result {
		private bool _success = true;

		public bool Success {
			get {
				return _success;
			}
			set {
				_success = value;
			}
		}

		public bool Failure {
			get {
				return !_success;
			}
			set {
				_success = !value;
			}
		}

		public string Message { get; set; }

		public static Result Ok(string message = "") {
			return new Result {
				Success = true,
				Message = message
			};
		}

		public static Result Error(string message) {
			return new Result {
				Success = false,
				Message = message
			};
		}
	}
}