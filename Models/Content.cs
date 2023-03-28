using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ArtShareServer.Models.Base;
using ArtShareServer.Models.DTOs;
using ArtShareServer.Models.Requests;

namespace ArtShareServer.Models
{
    public class Content : IModel, IComparable<Content>
    {
        public int Id { get; set; }
        [Required]
        public ContentTypesEnum ContentType { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int LikesCount { get; set; }
        public int DislikesCount { get; set; }
        public string ContentPath { get; set; }
        public List<ContentLike> Likes { get; set; }
        public List<ContentDislike> Dislikes { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Tag> Tags { get; set; }

        public Content()
        {
            Likes = new List<ContentLike>();
            Dislikes = new List<ContentDislike>();
            Comments = new List<Comment>();
            Tags = new List<Tag>();
        }

        public int CompareTo(Content other) {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return Id.CompareTo(other.Id);
        }
    }
}