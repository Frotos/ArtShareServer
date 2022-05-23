namespace ArtShareServer.Models {
  public class Session {
    public string Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public string Ip { get; set; }
    public string Created { get; set; }
    public string Last { get; set; }

    public void Copy(Session updatedSession) {
      UserId = updatedSession.UserId;
      User = updatedSession.User;
      Ip = updatedSession.Ip;
      Last = updatedSession.Last;
    }
  }
}