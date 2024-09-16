using Microsoft.AspNetCore.Routing.Constraints;
using System;
using System.ComponentModel.DataAnnotations;

namespace CETWebProject.Data.Entities
{
    public class Reading : IEntity
    {
        public int Id { get; set; }

        [Display(Name = "Amount of consumed water")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = false)]
        public double UsageAmount { get; set; }

        [Display(Name = "Reading time")]
        public DateTime ReadingTime { get; set; }
        
        public WaterMeter WaterMeter { get; set; }

    }
}