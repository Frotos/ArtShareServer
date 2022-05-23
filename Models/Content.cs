using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using ArtShareServer.Models.Base;
using Newtonsoft.Json;

namespace ArtShareServer.Models
{
    [DataContract(IsReference = true)]
    [JsonObject(IsReference = false)]
    public class Content : IModel, IComparable<Content>
    {
        [DataMember]
        public int Id { get; set; }
        public int UserId { get; set; }
        [DataMember]
        public User User { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public int LikesCount { get; set; }
        [NotMapped]
        [DataMember]
        public bool IsLikedByUser { get; set; }
        [DataMember]
        public int DislikesCount { get; set; }
        [NotMapped]
        [DataMember]
        public bool IsDislikedByUser { get; set; }
        [JsonIgnore]
        public string ContentPath { get; set; }
        [NotMapped]
        public string ContentBase64 { get; set; }
        [JsonIgnore]
        public List<ContentLike> Likes { get; set; }
        [JsonIgnore]
        public List<ContentDislike> Dislikes { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Vote> Votes { get; set; }
        public float AverageVote { get; set; }
        [DataMember]
        public List<Tag> Tags { get; set; }

        public Content()
        {
            Likes = new List<ContentLike>();
            Dislikes = new List<ContentDislike>();
            Comments = new List<Comment>();
            Votes = new List<Vote>();
            Tags = new List<Tag>();
        }

        public int CompareTo(Content other) {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return Id.CompareTo(other.Id);
        }

        // public object Clone(EFDBContext context)
        // {
        //     ImageContent image = new ImageContent()
        //     {
        //         Id = this.Id,
        //         User =  new User() {Id = this.User.Id, Username = this.User.Username, Avatar = this.User.Avatar},
        //         Title = this.Title,
        //         Description = this.Description,
        //         LikesCount = this.LikesCount,
        //         DislikesCount = this.DislikesCount,
        //         Content = this.Content,
        //         Likes = this.Likes,
        //         Dislikes = this.Dislikes,
        //         Comments = this.Comments,
        //         Tags = this.Tags
        //     };
        //     
        //     image.User.Followers = context.Followings
        //         .Include(u => u.FollowUser)
        //         .Include(u => u.FollowingUser)
        //         .Select(f => f)
        //         .Where(f => f.FollowUser.Id == image.User.Id)
        //         .ToList();
        //     
        //     if (image.User.Followers.Select(f => f).Any(f => f.FollowingUser.Id == this.User.Id))
        //     {
        //         image.User.IsFollowedByUser = true;
        //     }
        //     else
        //     {
        //         image.User.IsFollowedByUser = false;
        //     }
        //
        //     return image;
        // }
    }
}