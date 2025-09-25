using System.Net;

namespace PriceWebApi.Models.ApiResponse
{
    public class ApiResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public object Result { get; set; }
        public bool IsSuccess { get; set; }
        public List<string> ErrorMessage { get; set; }
    }
}