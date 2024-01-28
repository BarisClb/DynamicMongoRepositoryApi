namespace DynamicMongoRepositoryApi.WebApi.Models.Responses
{
    public sealed class ApiResponse<T> where T : class
    {
        public T Data { get; set; }
        public int StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public ApiResponseError Error { get; set; }


        public static ApiResponse<T> Success(T data, int statusCode)
        {
            return new ApiResponse<T> { Data = data, StatusCode = statusCode, IsSuccess = true };
        }

        public static ApiResponse<T> Fail(string error, int statusCode)
        {
            return new ApiResponse<T> { Data = default, StatusCode = statusCode, IsSuccess = false, Error = new() { ApiErrorMessage = error, Exception = null, ExceptionMessage = null, RequestBody = null } };
        }
    }

    public class ApiResponseError
    {
        public string ApiErrorMessage { get; set; }
        public string Exception { get; set; }
        public string ExceptionMessage { get; set; }
        public string RequestBody { get; set; }
    }

    public class NoContentResponse
    { }
}
