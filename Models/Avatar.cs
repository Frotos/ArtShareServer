using ArtShareServer.Models.Base;

namespace ArtShareServer.Models
{
    public class Avatar : IModel
    {
        public int Id { get; set; }
        public string Image { get; set; }

        public void Copy(Avatar avatar) {
            Image = avatar.Image;
        }
    }
}