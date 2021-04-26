using Microsoft.AspNetCore.Http;
using powerful_crm.Core;
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
            catch (CustomException ex)
            {
                await HandleCustomExceptionAsync(httpContext, ex);
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

        private Task HandleCustomExceptionAsync(HttpContext context, CustomException exception)
        {
            ModifyContextResponse(context, exception.StatusCode);
            return ConstructResponse(context, exception.StatusCode, exception.ErrorMessage);
        }
        private Task HandleSqlExceptionAsync(HttpContext context, SqlException exception)
        {
            var keys = new string[] { Constants.EMAIL_UNIQUE_CONSTRAINT, Constants.LOGIN_UNIQUE_CONSTRAINT };
            var result = keys.FirstOrDefault<string>(s => exception.Message.Contains(s));
            switch (result)
                {
                case Constants.LOGIN_UNIQUE_CONSTRAINT:
                    ModifyContextResponse(context, (int)HttpStatusCode.Conflict);
                    return ConstructResponse(context, 409, Constants.ERROR_NOT_UNIQUE_LOGIN);
                case Constants.EMAIL_UNIQUE_CONSTRAINT:
                    ModifyContextResponse(context, (int)HttpStatusCode.Conflict);
                    return ConstructResponse(context, 409, Constants.ERROR_NOT_UNIQUE_EMAIL);
                default:
                    return HandleExceptionAsync(context, exception);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            ModifyContextResponse(context, (int)HttpStatusCode.BadRequest);
            return ConstructResponse(context, (int)HttpStatusCode.BadRequest, Constants.GLOBAL_ERROR_MESSAGE);
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
