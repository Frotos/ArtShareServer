using ArtShareServer.Models.Base;

namespace ArtShareServer.Models
{
    public class ContentReport : IModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int ContentId { get; set; }
        public Content Content { get; set; }

        public ContentReport()
        {
        }

        public ContentReport(User user, Content content)
        {
            User = user;
            Content = content;
        }

        public void Copy(ContentReport updatedReport) {
            User = updatedReport.User;
            Content = updatedReport.Content;
        }
    }
}