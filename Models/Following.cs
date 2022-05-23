using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using ArtShareServer.Models.Base;

namespace ArtShareServer.Models
{
    public class Following : IModel
    {
        public int Id { get; set; }

        public int FollowUserId { get; set; }
        
        /// <summary>
        /// User who is being followed
        /// </summary>
        [ForeignKey("FollowUserId")]
        public User FollowUser { get; set; }
        
        public int FollowingUserId { get; set; }
        
        /// <summary>
        /// User who is following
        /// </summary>
        [ForeignKey("FollowingUserId")]
        public User FollowingUser { get; set; }

        public Following()
        {
        }
    }
}