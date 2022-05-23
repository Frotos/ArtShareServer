using ArtShareServer.Models.Base;

namespace ArtShareServer.Models
{
    public class CommentReport : IModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int CommentId { get; set; }
        public Comment Comment { get; set; }

        public CommentReport()
        {
        }

        public CommentReport(User user, Comment comment)
        {
            User = user;
            Comment = comment;
        }

        public void Copy(CommentReport updatedReport) {
            User = updatedReport.User;
            Comment = updatedReport.Comment;
        }
    }
}