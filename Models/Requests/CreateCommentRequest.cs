namespace ArtShareServer.Models.Requests {
  public class CreateCommentRequest {
    public int ContentId { get; set; }
    public string CommentText { get; set; }
  }
}