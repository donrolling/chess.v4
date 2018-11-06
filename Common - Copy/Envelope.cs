namespace common {

	public class Envelope<T> : Result {
		public T Result { get; set; }
		
		public static Envelope<T> Ok(T output, string message = "") {
			return new Envelope<T> {
				Success = true,
				Message = message,
				Result = output
			};
		}

		public new static Envelope<T> Error(string message) {
			return new Envelope<T> {
				Success = false,
				Message = message
			};
		}
	}
}