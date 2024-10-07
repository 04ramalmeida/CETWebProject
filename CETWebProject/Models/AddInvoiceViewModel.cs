using CETWebProject.Data.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace CETWebProject.Models
{
    public class AddInvoiceViewModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        [Display(Name = "Amount used")]
        public double Value { get; set; }

        public DateTime Date { get; set; }
    }
}
