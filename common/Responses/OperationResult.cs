using Common.Enums;

namespace Common.Responses
{
    public class OperationResult<T> : OperationResult
    {
        /// <summary>
        /// Contains the results of a successful call.  Can be null
        /// </summary>
        public T Result { get; set; }
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
    }
}