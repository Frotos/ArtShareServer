using System;

namespace ArtShareServer.Exceptions {
  public class BadRequestHttpException : HttpException {
    public override int StatusCode { get; } = 400;
    
    public BadRequestHttpException() {}
    public BadRequestHttpException(string message) : base(message) {}

    public BadRequestHttpException(string message, Exception innerException) : base(message, innerException) {}
  }
}