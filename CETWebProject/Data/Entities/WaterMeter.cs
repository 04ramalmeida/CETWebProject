using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CETWebProject.Data.Entities
{
    public class WaterMeter
    {
        public int Id { get; set; }

        public IEnumerable<Reading> Readings { get; set; }

        [Required]
        public User User { get; set; }
    }
}
