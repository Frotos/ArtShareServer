using System;

namespace ArtShareServer.Exceptions {
  public class CommentNotFoundException : Exception {
    public CommentNotFoundException() {
    }

    public CommentNotFoundException(string message) : base(message) {
    }

    public CommentNotFoundException(string message, Exception innerException) : base(message, innerException){
    }
  }
}