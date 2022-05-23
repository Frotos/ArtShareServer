using System;

namespace ArtShareServer.Exceptions {
  public class EmailIsTakenException : Exception {
    public EmailIsTakenException() {
    }

    public EmailIsTakenException(string message) : base(message) {
    }

    public EmailIsTakenException(string message, Exception innerException) : base(message, innerException) {
    }
  }
}