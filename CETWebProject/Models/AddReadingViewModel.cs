using System;

namespace CETWebProject.Models
{
    public class AddReadingViewModel
    {
        public int Id { get; set; }

        public float UsageAmount { get; set; }

        public DateTime ReadingTime { get; set; }

        public int WaterMeterId { get; set; }
    }
}
