namespace ArtShareServer.Models {
  public class Session {
    public int Id { get; set; }
    public string Token { get; set; }
    public string Ip { get; set; }
    public string Created { get; set; }
    public string Last { get; set; }

    public void Copy(Session updatedSession) {
      Ip = updatedSession.Ip;
      Last = updatedSession.Last;
    }
  }
}