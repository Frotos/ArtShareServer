using System;

namespace ArtShareServer.Exceptions {
  public class UnauthorizedHttpException : HttpException {
    public override int StatusCode { get; } = 401;
    
    public UnauthorizedHttpException() {}

    public UnauthorizedHttpException(string message) : base(message) {}

    public UnauthorizedHttpException(string message, Exception innerException) : base(message, innerException) {}
  }
}