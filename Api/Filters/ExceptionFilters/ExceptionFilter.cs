using Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;


namespace Api.Filters.ExceptionFilters;

public class ExceptionFilter : IExceptionFilter
{
    private readonly ProblemDetailsFactory _problemDetailsFactory;
    private readonly IHttpContextAccessor _contextAccessor;

    public ExceptionFilter(ProblemDetailsFactory problemDetailsFactory, IHttpContextAccessor contextAccessor)
    {
        _problemDetailsFactory = problemDetailsFactory;
        _contextAccessor = contextAccessor;
    }

    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        var problemDetails = new ProblemDetails();

        if (exception is SavingChangesFailedException)
        {
            problemDetails = CreateProblemDetails(500, "Database saving failed", exception.Message);
        }
        else
        {
            return;
        }

        context.ExceptionHandled = true;
        context.Result = new ObjectResult(problemDetails);
    }

    private ProblemDetails CreateProblemDetails(int statusCode, string title, string detail)
    {
        HttpContext context = _contextAccessor.HttpContext!;
        return _problemDetailsFactory.CreateProblemDetails(context, statusCode, title, detail: detail);
    }
}
