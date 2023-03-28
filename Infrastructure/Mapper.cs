using System.Collections.Generic;
using ArtShareServer.Models;
using ArtShareServer.Models.DTOs;

namespace ArtShareServer.Infrastructure {
  public class Mapper {
    public AvatarDto MapAvatarToDto(Avatar avatar) {
      return new AvatarDto() {Image = avatar.Image};
    }

    public UserDto MapUserToDto(User user, bool isFollowed) {
      return new UserDto() {
        Id = user.Id,
        Username = user.Username,
        Avatar = MapAvatarToDto(user.Avatar),
        FollowersCount = user.FollowersCount,
        FollowingsCount = user.FollowingsCount,
        PostsCount = user.PostsCount,
        IsFollowed = isFollowed
      };
    }

    public List<TagDto> MapTagsToDto(List<Tag> tags) {
      var result = new List<TagDto>(tags.Count);

      foreach (var tag in tags) {
        result.Add(new TagDto() {Name = tag.Name});
      }
      
      return result;
    }

    public ContentDto MapContentToDto(Content content, bool isLiked, bool isDisliked, bool isFollowed, string contentBase64, bool isUploadedByUser) {
      return new ContentDto() {
        Id = content.Id,
        Type = content.ContentType,
        User = MapUserToDto(content.User, isFollowed),
        Title = content.Title,
        Description = content.Description,
        LikesCount = content.LikesCount,
        DislikesCount = content.DislikesCount,
        IsLiked = isLiked,
        IsDisliked = isDisliked,
        ContentBase64 = contentBase64,
        UploadedByUser = isUploadedByUser,
        Tags = MapTagsToDto(content.Tags)
      };
    }

    public Avatar MapDtoToAvatar(AvatarDto dto) {
      return dto == null ? new Avatar() : new Avatar() {Image = dto.Image};
    }

    public User MapDtoToUser(UserDto dto) {
      return new User() {
        Username = dto.Username,
        Avatar = MapDtoToAvatar(dto.Avatar),
        FollowersCount = dto.FollowersCount,
        FollowingsCount = dto.FollowingsCount,
        PostsCount = dto.PostsCount,
      };
    }

    public Content MapDtoToContent(ContentDto dto) {
      return new Content() {
        ContentType = dto.Type,
        User = MapDtoToUser(dto.User),
        Title = dto.Title,
        Description = dto.Description,
        LikesCount = dto.LikesCount,
        DislikesCount = dto.DislikesCount,
      };
    }

    public CommentDto MapCommentToDto(Comment comment, bool isLiked, bool isDisliked, bool uploadedByUser) {
      return new CommentDto() {
        Id = comment.Id,
        Text = comment.CommentText,
        User = MapUserToDto(comment.User, false),
        IsLiked = isLiked,
        IsDisliked = isDisliked,
        LikesCount = comment.LikesCount,
        DislikesCount = comment.DislikesCount,
        UploadedByUser = uploadedByUser
      };
    }
  }
}