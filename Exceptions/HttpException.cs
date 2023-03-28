using System;

namespace ArtShareServer.Exceptions {
  public abstract class HttpException : Exception {
    public abstract int StatusCode { get; }

    public HttpException() {}

    public HttpException(string message) : base(message) {}

    public HttpException(string message, Exception innerException) : base(message, innerException) {}
  }
}