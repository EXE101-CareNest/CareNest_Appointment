using CareNest_Appointment.Domain.Commons.Constant;

namespace CareNest_Appointment.Domain.Commons.Base
{
    public class BaseException : Exception
    {
        public class CoreException : Exception
        {
            public CoreException(string code, string message, int statusCode = (int)StatusCodeHelper.ServerError)
                : base(message)
            {
                Code = code;
                StatusCode = statusCode;
            }


            public string Code { get; }

            public int StatusCode { get; set; }

            public Dictionary<string, object>? AdditionalData { get; set; }
        }

        public class BadRequestException : ErrorException
        {
            public BadRequestException(string errorCode, string message)
                : base(400, errorCode, message)
            {
            }
            public BadRequestException(ICollection<KeyValuePair<string, ICollection<string>>> errors)
                : base(400, new ErrorDetail
                {
                    ErrorCode = "bad_request",
                    ErrorMessage = errors
                })
            {
            }
        }

        public class ErrorException : Exception
        {
            public int StatusCode { get; }

            public ErrorDetail ErrorDetail { get; }

            public ErrorException(int statusCode, string errorCode, string message)
            {
                StatusCode = statusCode;
                ErrorDetail = new ErrorDetail
                {
                    ErrorCode = errorCode,
                    ErrorMessage = message
                };
            }

            public ErrorException(int statusCode, ErrorDetail errorDetail)
            {
                StatusCode = statusCode;
                ErrorDetail = errorDetail;
            }

        }
        public class UnauthorizedException : Exception
        {
            public UnauthorizedException(string message) : base(message) { }
        }

        public class ErrorDetail
        {
            public string? ErrorCode { get; set; }

            public object? ErrorMessage { get; set; }
        }

        public static ErrorException BadRequestInvaildInputResponse(string mess)
        {
            return new ErrorException((int)StatusCodeHelper.BadRequest, MessageConstant.INVALID_INPUT, mess);
        }

        public static ErrorException BadRequestNotFoundResponse(string mess)
        {
            return new ErrorException((int)StatusCodeHelper.BadRequest, MessageConstant.NotFound, mess);
        }

        public static ErrorException BadRequestBadRequestResponse(string mess)
        {
            return new ErrorException((int)StatusCodeHelper.BadRequest, MessageConstant.BadRequest, mess);
        }

        public static ErrorException BadRequestNotFoundResponse()
        {
            return new ErrorException((int)StatusCodeHelper.BadRequest, MessageConstant.NotFound, MessageConstant.NotFound);
        }

        public static ErrorException BadRequestDupplicationResponse(string mess)
        {
            return new ErrorException((int)StatusCodeHelper.BadRequest, MessageConstant.DuplicateRecord, mess);
        }

        public static ErrorException UnauthorizedUnAuthorizedResponse()
        {
            return new ErrorException((int)StatusCodeHelper.Unauthorized, MessageConstant.Unauthorized, MessageConstant.Unauthorized);
        }
    }
}
