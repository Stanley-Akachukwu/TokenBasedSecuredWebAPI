using System;

namespace UCare.Models.Entities
{
    public class Moderator
    {
        public Guid ModeratorId { get; set; }
        public User User { get; set; }
    }
}