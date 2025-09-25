using System.Net;
using PriceWebApi.Models.ApiResponse;

namespace PriceWebApi.Helpers
{
    public static class ResponseExtension
    {
        public static ApiResponse CreateApiResponse(this object result, bool isSuccess = true, HttpStatusCode statusCode = HttpStatusCode.OK, List<string> errorMessage = null)
        {
            if (result == null)
            {
                isSuccess = false;
                statusCode = HttpStatusCode.NotFound;
                errorMessage = ["Object Not Found"];
            }
            else if (result is IEnumerable<object> collection && !collection.Any())
            {
                isSuccess = false;
                statusCode = HttpStatusCode.NotFound;
                errorMessage = ["No Store At That Location"];
            }
            return new ApiResponse()
            {
                Result = result,
                IsSuccess = isSuccess,
                StatusCode = statusCode,
                ErrorMessage = errorMessage
            };
        }
    }
}