using ArtShareServer.Models.Base;

namespace ArtShareServer.Models
{
    public class CommentDislike : IModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int CommentId { get; set; }
        public Comment Comment { get; set; }
    }
}