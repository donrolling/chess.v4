namespace common {

	public class Result {
		private bool _success = true;

		public bool Sucess {
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
				Sucess = true,
				Message = message
			};
		}

		public static Result Error(string message) {
			return new Result {
				Sucess = false,
				Message = message
			};
		}
	}
}