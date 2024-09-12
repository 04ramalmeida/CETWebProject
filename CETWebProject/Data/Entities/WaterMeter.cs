using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CETWebProject.Data.Entities
{
    public class WaterMeter : IEntity
    {
        public int Id { get; set; }

        public ICollection<Reading> Readings { get; set; } = new List<Reading>();

        public User User { get; set; }
    }
}
