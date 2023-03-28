using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ArtShareServer.Models.DTOs {
  [DataContract(IsReference = true)]
  [JsonObject(IsReference = false)]
  public class ContentDto {
    public int Id { get; set; }
    public ContentTypesEnum Type { get; set; }
    public UserDto User { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int LikesCount { get; set; }
    public int DislikesCount { get; set; }
    public bool IsLiked { get; set; }
    public bool IsDisliked { get; set; }
    public string ContentBase64 { get; set; }
    public bool UploadedByUser { get; set; }
    public List<TagDto> Tags { get; set; }
  }
}