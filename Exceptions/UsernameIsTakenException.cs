using System;

namespace ArtShareServer.Exceptions {
  public class UsernameIsTakenException : Exception {
    public UsernameIsTakenException() {
    }

    public UsernameIsTakenException(string message) : base(message) {
    }

    public UsernameIsTakenException(string message, Exception innerException) : base(message, innerException) {
    }
  }
}