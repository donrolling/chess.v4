using Common.Enums;
using Common.Responses;

namespace Common.Factories
{
	public static class OperationResultFactory
	{
		public static OperationResult Create(bool success, string message)
		{
			return new OperationResult {
				Success = success,
				Message = message
			};
		}

		public static OperationResult Create(bool success, string message, Status status)
		{
			return new OperationResult {
				Success = success,
				Message = message,
				Status = status
			};
		}

		public static OperationResult<T> Create<T>(OperationResult copyFrom)
		{
			var x = new OperationResult<T>();
			foreach (var propInf in copyFrom.GetType().GetProperties())
			{
				propInf.SetValue(x, propInf.GetValue(copyFrom));
			}
			return x;
		}

		public static OperationResult<T> Create<T>(OperationResult copyFrom, T result)
		{
			var x = Create<T>(copyFrom);
			x.Result = result;
			return x;
		}

		public static OperationResult Fail(string message)
		{
			return OperationResultFactory.Create(false, message, Status.Failure);
		}

		public static OperationResult Fail(Status status, string message)
		{
			return OperationResultFactory.Create(false, message, status);
		}

		public static OperationResult Fail(Status status = Status.Failure)
		{
			return OperationResultFactory.Create(false, string.Empty, status);
		}

		public static OperationResult<T> Fail<T>(T result, Status status = Status.Failure, string message = "")
		{
			return OperationResultFactory.Create<T>(OperationResultFactory.Fail(status, message), result);
		}

		public static OperationResult<T> Fail<T>(string message)
		{
			return OperationResultFactory.Create<T>(OperationResultFactory.Fail(Status.Failure, message));
		}

		public static OperationResult Ok(Status status = Status.Success, string message = "")
		{
			return OperationResultFactory.Create(true, message, status);
		}

		public static OperationResult<T> Ok<T>(T result, string message = "")
		{
			return OperationResultFactory.Create<T>(OperationResultFactory.Ok(Status.Success, message), result);
		}

		public static OperationResult<T> Ok<T>(T result, Status status, string message = "")
		{
			return OperationResultFactory.Create<T>(OperationResultFactory.Ok(status, message), result);
		}
	}
}