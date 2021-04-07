using Microsoft.AspNetCore.Http;
using powerful_crm.Core.CustomExceptions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;

namespace powerful_crm.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private const string GlobalErrorMessage = "An error occured while processing the request.";
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (ValidationException ex)
            {
                await HandleValidationExceptionAsync(httpContext, ex);
            }
            catch (SqlException ex)
            {
                await HandleSqlExceptionAsync(httpContext, ex);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
        {
            ModifyContextResponse(context, exception.StatusCode);

            return ConstructResponse(context, exception.StatusCode, exception.ErrorMessage);
        }
        private Task HandleSqlExceptionAsync(HttpContext context, SqlException exception)
        {
            ModifyContextResponse(context, (int)HttpStatusCode.BadRequest);
            string errorMessage;
            int statusCode;
            if (exception.Message.Contains("UQLead5E55825B7B2276C4"))
            {
                errorMessage = "This login is already in use.";
                statusCode = 409;
            }
            else if (exception.Message.Contains("UQLeadA9D10534BF185160"))
            {
                errorMessage = "This email is already in use.";
                statusCode = 409;
            }
            else
            {
                errorMessage = GlobalErrorMessage;
                statusCode = 400;
            }
            return ConstructResponse(context, statusCode, errorMessage);
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            ModifyContextResponse(context, (int)HttpStatusCode.BadRequest);

            return ConstructResponse(context, (int)HttpStatusCode.BadRequest, GlobalErrorMessage);
        }

        private void ModifyContextResponse(HttpContext context, int statusCode)
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = statusCode;
        }

        private Task ConstructResponse(HttpContext context, int statusCode, string message)
        {
            var errorResponse = JsonSerializer.Serialize(
                new
                {
                    Code = statusCode,
                    Message = message
                });

            return context.Response.WriteAsync(errorResponse);
        }
    }
}
