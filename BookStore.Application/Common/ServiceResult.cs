namespace BookStore.Application.Common;

public class ServiceResult<T>
{
    // Private constructor: Forces usage of static factory methods (Success/Failure)
    private ServiceResult(T? data, ServiceError? error, bool isSuccess)
    {
        Data = data;
        Error = error;
        IsSuccess = isSuccess;
    }

    public T? Data { get; }
    public ServiceError? Error { get; }
    public bool IsSuccess { get; }

    // Factory Method for Success
    // Usage: return ServiceResult<Book>.Success(myBook);
    public static ServiceResult<T> Success(T data) 
        => new(data, null, true);

    // Factory Method for Failure
    // Usage: return ServiceResult<Book>.Failure(new ServiceError("Not Found", ...));
    public static ServiceResult<T> Failure(ServiceError error) 
        => new(default, error, false);
}