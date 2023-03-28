using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ArtShareServer.Models.Base;

namespace ArtShareServer.Models {
  public class User : IModel {
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    [NotNull] public string Password { get; set; }
    public int AvatarId { get; set; }
    public Avatar Avatar { get; set; }
    public List<ContentLike> Likes { get; set; }
    public List<ContentDislike> Dislikes { get; set; }
    public List<CommentLike> CommentLikes { get; set; }
    public List<CommentDislike> CommentDislikes { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingsCount { get; set; }
    public int PostsCount { get; set; }
    public List<Following> Followings { get; set; }
    public List<Following> Followers { get; set; }
    public List<ContentReport> ContentReports { get; set; }
    public List<CommentReport> CommentReports { get; set; }
    public List<Comment> Comments { get; set; }

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