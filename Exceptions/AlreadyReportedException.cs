using System;

namespace ArtShareServer.Exceptions {
  public class AlreadyReportedException : Exception {
    public AlreadyReportedException() {}

    public AlreadyReportedException(string message) : base(message) {}
    
    public AlreadyReportedException(string message, Exception innerException) : base(message, innerException) {}
  }
}