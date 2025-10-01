
using CareNest_Appointment.Domain.Commons.Utils;

namespace CareNest_Appointment.Domain.Commons.Constant
{
    public enum StatusCodeHelper
    {
        [CustomName("Success")]
        OK = 200,

        [CustomName("Bad Request")]
        BadRequest = 400,

        [CustomName("Unauthorized")]
        Unauthorized = 401,

        [CustomName("Internal Server Error")]
        ServerError = 500
    }
}
