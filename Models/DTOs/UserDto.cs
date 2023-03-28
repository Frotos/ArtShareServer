using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ArtShareServer.Models.DTOs {
  [DataContract(IsReference = true)]
  [JsonObject(IsReference = false)]
  public class UserDto {
    public int Id { get; set; }
    public string Username { get; set; }
    public AvatarDto Avatar { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingsCount { get; set; }
    public int PostsCount { get; set; }
    public bool IsFollowed { get; set; }
  }
}