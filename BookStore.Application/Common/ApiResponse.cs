namespace BookStore.Application.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }

    // Constructor for Success
    public ApiResponse(T? data, string message = "Success")
    {
        Success = true;
        Message = message;
        Data = data;
        Errors = null;
    }

    // Constructor for Failure
    public ApiResponse(List<string> errors, string message = "Failure")
    {
        Success = false;
        Message = message;
        Data = default;
        Errors = errors;
    }
}