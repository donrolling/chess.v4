namespace common {

	public class ResultOuput<T> : Result {
		public T Output { get; set; }
		
		public static ResultOuput<T> Ok(T output, string message = "") {
			return new ResultOuput<T> {
				Sucess = true,
				Message = message,
				Output = output
			};
		}

		public new static ResultOuput<T> Error(string message) {
			return new ResultOuput<T> {
				Sucess = false,
				Message = message
			};
		}
	}
}