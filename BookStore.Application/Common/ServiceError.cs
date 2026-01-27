namespace BookStore.Application.Common;

public record ServiceError(string Message, ServiceErrorType Type);

public enum ServiceErrorType
{
    NotFound = 404,
    Validation = 400,
    Conflict = 409, // Duplicate entry
    Unauthorized = 401,
    Internal = 500
}