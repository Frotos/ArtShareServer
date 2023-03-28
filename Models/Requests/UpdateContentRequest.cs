using System.Collections.Generic;
using ArtShareServer.Models.DTOs;

namespace ArtShareServer.Models.Requests {
  public class UpdateContentRequest {
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public List<TagDto> Tags { get; set; }
  }
}