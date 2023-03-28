using System;

namespace ArtShareServer.Exceptions {
  public class NotFoundHttpException : HttpException {
    public override int StatusCode { get; } = 404;
    
    public NotFoundHttpException() {}

    public NotFoundHttpException(string message) : base(message) {}

    public NotFoundHttpException(string message, Exception innerException) : base(message, innerException) {}
  }
}