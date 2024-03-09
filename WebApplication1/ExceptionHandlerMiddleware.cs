using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MyMakler
{
    public class DoesNotExistException : Exception 
    {
        public DoesNotExistException(Type entityType)
        {
            if (entityType == typeof(Advertisement))
                Message = "Advertisement does not exists";
            else if (entityType == typeof(User))
                Message = "User does not exists";
            else if (entityType == typeof(File))
                Message = "File does not exists";
            else throw new Exception();
        }
        public new string Message;
    }
    public class InvalidPageException : Exception
    {
        public string Message = "Page number is invalid or does not exist";
    }
    public class InvalidFileFormatException : Exception
    {
        public string Message = "Sent file is not a picture";
    }
    public class EmptyFileException : Exception
    {
        public string Message = "File was not sent";
    }
    public class TooManyAdsException : Exception
    {
        public string Message = "You can not create more ads. Try to delete old one.";
    }

    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (TooManyAdsException tMAEx)
            {
                await HandleTooManyAdsException(context, tMAEx);
            }
            catch (EmptyFileException eFEx)
            {
                await HandleEmptyFileException(context, eFEx);
            }
            catch (InvalidFileFormatException iFFEx)
            {
                await HandleInvalidFileFormatException(context, iFFEx);
            }
            catch (InvalidPageException iPEx)
            {
                await HandleInvalidPageException(context, iPEx);
            }
            catch (DoesNotExistException dNEEx)
            {
                await HandleDoesNotExistException(context, dNEEx);
            }
            catch (Exception ex)
            {
                await HandleUnknownException(context, ex);
            }
        }
        private static Task HandleTooManyAdsException(HttpContext context, TooManyAdsException exception)
        {
            int statusCode = (int)HttpStatusCode.Forbidden;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            var result = JsonConvert.SerializeObject(new
            {
                StatusCode = statusCode,
                ErrorMessage = exception.Message
            });
            return context.Response.WriteAsync(result);
        }
        private static Task HandleEmptyFileException(HttpContext context, EmptyFileException exception)
        {
            int statusCode = 418;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            var result = JsonConvert.SerializeObject(new
            {
                StatusCode = statusCode,
                ErrorMessage = exception.Message
            });
            return context.Response.WriteAsync(result);
        }
        private static Task HandleInvalidFileFormatException(HttpContext context, InvalidFileFormatException exception)
        {
            int statusCode = (int)HttpStatusCode.UnsupportedMediaType;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            var result = JsonConvert.SerializeObject(new
            {
                StatusCode = statusCode,
                ErrorMessage = exception.Message
            });
            return context.Response.WriteAsync(result);
        }
        private static Task HandleInvalidPageException(HttpContext context, InvalidPageException exception)
        {
            int statusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            var result = JsonConvert.SerializeObject(new
            {
                StatusCode = statusCode,
                ErrorMessage = exception.Message
            });
            return context.Response.WriteAsync(result);
        }
        private static Task HandleDoesNotExistException(HttpContext context, DoesNotExistException exception)
        {
            int statusCode = (int)HttpStatusCode.NotFound;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            var result = JsonConvert.SerializeObject(new
            {
                StatusCode = statusCode,
                ErrorMessage = exception.Message
            });
            return context.Response.WriteAsync(result);
        }
        private static Task HandleUnknownException(HttpContext context, Exception exception)
        {
            int statusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            var result = JsonConvert.SerializeObject(new
            {
                StatusCode = statusCode,
                ErrorMessage = exception.Message
            });
            return context.Response.WriteAsync(result);
        }
    }
    public static class ExceptionHandlerMiddlewareExtensions
    {
        public static void UseExceptionHandlerMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}
