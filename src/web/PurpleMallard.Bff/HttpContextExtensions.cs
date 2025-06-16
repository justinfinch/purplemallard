using System;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace PurpleMallard.Bff;

internal static class HttpContextExtensions
{
    public static void ReturnHttpProblem(this HttpContext context, string title, params (string key, string[] values)[] errors)
    {
        var problem = new HttpValidationProblemDetails()
        {
            Status = (int)HttpStatusCode.BadRequest,
            Title = title,
            Errors = errors.ToDictionary()
        };
        context.Response.StatusCode = problem.Status.Value;
        context.Response.ContentType = "application/problem+json";
        context.Response.WriteAsJsonAsync(problem);

    }
}
