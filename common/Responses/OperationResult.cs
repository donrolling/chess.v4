using Common.Enums;

namespace Common.Responses
{
    public class OperationResult<T> : OperationResult
    {
        /// <summary>
        /// Contains the results of a successful call.  Can be null
        /// </summary>
        public T Result { get; set; }

        public OperationResult()
        {
        }

        public OperationResult(OperationResult copyFrom, T result)
        {
            foreach (var propInf in copyFrom.GetType().GetProperties())
            {
                propInf.SetValue(this, propInf.GetValue(copyFrom));
            }
            Result = result;
        }

        public OperationResult(OperationResult copyFrom)
        {
            foreach (var propInf in copyFrom.GetType().GetProperties())
            {
                propInf.SetValue(this, propInf.GetValue(copyFrom));
            }
        }

        public static OperationResult<T> Ok(T result, Status status = Status.Success, string message = "")
        {
            var x = new OperationResult<T>(OperationResult.Ok(status, message));
            x.Result = result;
            return x;
        }

        public static OperationResult<T> Fail(T result, Status status = Status.Failure, string message = "")
        {
            var x = new OperationResult<T>(OperationResult.Fail(status, message));
            x.Result = result;
            return x;
        }

        public static OperationResult<T> Fail(string message)
        {
            var x = new OperationResult<T>(OperationResult.Fail(Status.Failure, message));
            return x;
        }
    }

    public class OperationResult
    {
        public bool Failure
        {
            get => _failure;
            set
            {
                _failure = value;
                //this is here to make the success and failure values agree with one another
                _success = !_failure;
                //this is here to prevent the status to disagree with success and failure settings
                if (_failure && _status == Status.Success)
                {
                    _status = Status.Failure;
                }
            }
        }

        public string Message { get; set; }

        public Status Status
        {
            get => _status;
            set => _status = value;
        }

        public bool Success
        {
            get => _success;
            set
            {
                _success = value;
                //this is here to make the success and failure values agree with one another
                _failure = !_success;
                //this is here to prevent the status to disagree with success and failure settings
                if (!_success && _status == Status.Success)
                {
                    _status = Status.Failure;
                }
            }
        }

        private bool _failure;
        private Status _status = Status.Success;
        private bool _success = true;

        public OperationResult()
        {
        }

        public OperationResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public OperationResult(bool success, string message, Status status)
        {
            Success = success;
            Message = message;
            Status = status;
        }

        public static OperationResult Fail(string message)
        {
            return new OperationResult(false, message, Status.Failure);
        }

        public static OperationResult Fail(Status status, string message)
        {
            return new OperationResult(false, message, status);
        }

        public static OperationResult Fail(Status status = Status.Failure)
        {
            return new OperationResult(false, string.Empty, status);
        }

        public static OperationResult Ok(Status status = Status.Success, string message = "")
        {
            return new OperationResult(true, message, status);
        }
    }
}