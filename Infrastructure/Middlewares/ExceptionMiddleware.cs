using System;
using System.Net;
using System.Threading.Tasks;
using ArtShareServer.Exceptions;
using ArtShareServer.Middlewares.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ArtShareServer.Infrastructure.Middlewares {
  public class ExceptionMiddleware {
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next) {
      _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext) {
      try {
        await _next(httpContext);
      }
      catch (Exception e) {
        await HandleExceptionAsync(httpContext, e);
      }
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception) {
      var statusCode = (int)HttpStatusCode.InternalServerError;

      if (exception is HttpException httpException) {
        statusCode = httpException.StatusCode;
      }

      var responseModel = JsonConvert.SerializeObject(new ErrorResponseModel() {Message = exception.Message});
      httpContext.Response.ContentType = "application/json";
      httpContext.Response.StatusCode = statusCode;
      await httpContext.Response.WriteAsync(responseModel);
    }
  }
}