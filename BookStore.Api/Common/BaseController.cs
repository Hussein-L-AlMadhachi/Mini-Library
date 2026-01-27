using BookStore.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Common;


[ApiController]
[Route("/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected ActionResult HandleResult<T>(ServiceResult<T> result)
    {
        if (result.IsSuccess)
        {
            // Wrap the data in ApiResponse
            var response = new ApiResponse<T>(result.Data);
            return Ok(response);
        }

        // Wrap the error
        var errorResponse = new ApiResponse<object>(
            new List<string> { result.Error.Message }, 
            "Operation Failed"
        );

        return result.Error.Type switch
        {
            ServiceErrorType.NotFound => NotFound(errorResponse),
            ServiceErrorType.Validation => BadRequest(errorResponse),
            ServiceErrorType.Conflict => Conflict(errorResponse),
            _ => StatusCode(500, errorResponse)
        };
    }
}