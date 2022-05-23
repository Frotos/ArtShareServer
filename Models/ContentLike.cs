using ArtShareServer.Models.Base;

namespace ArtShareServer.Models
{
    public class ContentLike : IModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int ContentId { get; set; }
        public Content Content { get; set; }
    }
}