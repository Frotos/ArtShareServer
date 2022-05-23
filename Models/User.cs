using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using ArtShareServer.Models.Base;
using Newtonsoft.Json;

namespace ArtShareServer.Models {
  [DataContract(IsReference = true)]
  [JsonObject(IsReference = false)]
  public class User : IModel {
    [DataMember] public int Id { get; set; }
    [DataMember] public string Username { get; set; }
    [DataMember] public string Email { get; set; }
    [DataMember] [NotNull] public string Password { get; set; }
    public int AvatarId { get; set; }
    [DataMember] public Avatar Avatar { get; set; }
    [JsonIgnore] public List<ContentLike> Likes { get; set; }
    [JsonIgnore] public List<ContentDislike> Dislikes { get; set; }
    [JsonIgnore] public List<CommentLike> CommentLikes { get; set; }
    [JsonIgnore] public List<CommentDislike> CommentDislikes { get; set; }
    [DataMember] public int FollowersCount { get; set; }
    [DataMember] public int FollowingsCount { get; set; }
    [DataMember] public int PostsCount { get; set; }
    [DataMember] [NotMapped] public bool IsFollowedByUser { get; set; }

    [JsonIgnore] public List<Following> Followings { get; set; }
    [JsonIgnore] public List<Following> Followers { get; set; }
    [JsonIgnore] public List<ContentReport> ContentReports { get; set; }
    [JsonIgnore] public List<CommentReport> CommentReports { get; set; }
    [JsonIgnore] public List<Vote> Votes { get; set; }

    public User() {
      Likes = new List<ContentLike>();
      Dislikes = new List<ContentDislike>();
      CommentLikes = new List<CommentLike>();
      CommentDislikes = new List<CommentDislike>();
      Followings = new List<Following>();
      Followers = new List<Following>();
    }

    public User(string username, string email, string password) {
      Username = username;
      Email = email;
      Password = password;
    }

    public void CopyValues(User user) {
      Username = user.Username;
      Email = user.Email;
      Password = user.Password;
      Avatar.Copy(user.Avatar);
    }
  }
}