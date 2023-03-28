using System;

namespace ArtShareServer.Exceptions {
  public class ForbiddenHttpException : HttpException {
    public override int StatusCode { get; } = 403;
    
    public ForbiddenHttpException() {}

    public ForbiddenHttpException(string message) : base(message) {}

    public ForbiddenHttpException(string message, Exception innerException) : base(message, innerException) {}
  }
}