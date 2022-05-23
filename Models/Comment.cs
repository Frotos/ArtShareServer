using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using ArtShareServer.Models.Base;

namespace ArtShareServer.Models
{
    [DataContract(IsReference = true)]
    public class Comment : IModel
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string CommentText { get; set; }

        public int UserId { get; set; }
        
        [DataMember]
        public User User { get; set; }
        [DataMember]
        public int ImageContentId { get; set; }

        public Content Content { get; set; }
        [DataMember]
        public string PublishDate { get; set; }
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

        public List<CommentLike> Likes { get; set; }
        public List<CommentDislike> Dislikes { get; set; }

        public Comment()
        {
            Likes = new List<CommentLike>();
            Dislikes = new List<CommentDislike>();
        }

        public void Copy(Comment comment) {
            CommentText = comment.CommentText;
            LikesCount = comment.LikesCount;
        }
    }
}