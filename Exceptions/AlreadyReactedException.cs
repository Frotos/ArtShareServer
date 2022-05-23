using System;

namespace ArtShareServer.Exceptions {
  public class AlreadyReactedException : Exception {
    public AlreadyReactedException() {}

    public AlreadyReactedException(string message) : base(message) {}

    public AlreadyReactedException(string message, Exception innerException) : base(message, innerException) {}
  }
}