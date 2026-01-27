using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BookStore.Api.ActionFilters;

public class LogActivityAttribute : ActionFilterAttribute
{
    private readonly ILogger<LogActivityAttribute> _logger;

    // Filters are tricky with DI. We usually use IFilterFactory or ServiceFilter.
    // For simplicity in teaching, we will use ServiceFilter in the controller.
    public LogActivityAttribute(ILogger<LogActivityAttribute> logger)
    {
        _logger = logger;
    }

    // Runs BEFORE the action
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var actionName = context.ActionDescriptor.DisplayName;
        var args = JsonSerializer.Serialize(context.ActionArguments);
        
        _logger.LogInformation("Starting Action {Action} with args {Args}", actionName, args);
    }

    // Runs AFTER the action
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        _logger.LogInformation("Finished Action {Action}", context.ActionDescriptor.DisplayName);
    }
}