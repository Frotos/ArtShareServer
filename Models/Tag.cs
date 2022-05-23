using System.Collections.Generic;
using ArtShareServer.Models.Base;

namespace ArtShareServer.Models
{
    public class Tag : IModel
    {
        public int Id { get; set; }
        public List<Content> Contents { get; set; } = new List<Content>();
        public string Name { get; set; }
    }
}