namespace UserService.Middleware
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        public ApiResponse(bool success, T? data, List<string>? errors = null)
        {
            Success = success;
            Data = data;
            Errors = errors;
        }

        public static ApiResponse<T> SuccessResponse(T data)
            => new ApiResponse<T>(true, data);

        public static ApiResponse<T> ErrorResponse(List<string> errors)
            => new ApiResponse<T>(false, default, errors);
    }
}
