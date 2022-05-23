namespace ArtShareServer.Models.Requests {
  public class ContentFiltersRequest {
    public string[] SearchPhrases { get; set; }
    public bool Favourites { get; set; }
    public int AuthorId { get; set; }
  }
}