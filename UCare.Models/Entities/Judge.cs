using System;

namespace UCare.Models.Entities
{ 
    public class Judge
    {
        public Guid JudgeId { get; set; }
        public User UserDetail { get; set; }
    }
}