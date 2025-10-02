namespace CareNest_Appointment.Application.Common
{
    public class ResponseResult<T>
    {
        /// <summary>
        /// Indicates whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Contains the data returned from the API if the operation was successful.
        /// </summary>
        public ApiResponse<T>? Data { get; set; }

        /// <summary>
        /// Contains an error message or additional information if the operation failed.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// An optional error code to help identify specific errors.
        /// </summary>
        public int? ErrorCode { get; set; }

        public ResponseResult() { }

        public ResponseResult(bool isSuccess, ApiResponse<T>? data = default, string? message = null, int? errorCode = null)
        {
            IsSuccess = isSuccess;
            Data = data;
            Message = message;
            ErrorCode = errorCode;
        }
    }
}
