using CETWebProject.Data.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace CETWebProject.Models
{
    public class RequestMeterViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}
