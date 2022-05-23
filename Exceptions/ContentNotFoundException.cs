using System;

namespace ArtShareServer.Exceptions {
  public class ContentNotFoundException : Exception {
    public ContentNotFoundException() {}

    public ContentNotFoundException(string message) : base(message) {}

    public ContentNotFoundException(string message, Exception innerException) : base(message, innerException) {}
  }
}