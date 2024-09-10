using Microsoft.AspNetCore.Routing.Constraints;
using System;
using System.ComponentModel.DataAnnotations;

namespace CETWebProject.Data.Entities
{
    public class Reading : IEntity
    {
        public int Id { get; set; }

        [Display(Name = "Amount of consumed water")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public double usageAmount { get; set; }

        [Display(Name = "Reading date")]
        public DateTime dataDeLeitura { get; set; }

    }
}