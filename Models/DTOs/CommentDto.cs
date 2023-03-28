namespace ArtShareServer.Models.DTOs {
  public class CommentDto {
    public int Id { get; set; }
    public string Text { get; set; }
    public UserDto User { get; set; }
    public bool IsLiked { get; set; }
    public bool IsDisliked { get; set; }
    public int LikesCount { get; set; }
    public int DislikesCount { get; set; }
    public bool UploadedByUser { get; set; }
  }
}