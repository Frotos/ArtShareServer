using System;

namespace ArtShareServer.Exceptions {
  public class SessionNotFoundException : Exception {
    public SessionNotFoundException() {
    }

    public SessionNotFoundException(string message) : base(message) {
    }
    
    public SessionNotFoundException(string message, Exception innerException) : base(message, innerException) {
    }
  }
}