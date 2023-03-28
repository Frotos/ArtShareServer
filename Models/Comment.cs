using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using ArtShareServer.Models.Base;

namespace ArtShareServer.Models
{
    [DataContract(IsReference = true)]
    public class Comment : IModel
    {
        public int Id { get; set; }
        public string CommentText { get; set; }

        public int UserId { get; set; }
        
        public User User { get; set; }
        public int ContentId { get; set; }

        public Content Content { get; set; }
        public string PublishDate { get; set; }
        public int LikesCount { get; set; }
        [DataMember]
        public int DislikesCount { get; set; }

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