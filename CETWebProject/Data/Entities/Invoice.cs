using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CETWebProject.Data.Entities
{
    public class Invoice : IEntity
    {
        public int Id { get; set; }

        public decimal TotalValue { get; set; }

        [DisplayName("First Echelon")]
        public decimal FirstDecimalValue { get; set; }

        [DisplayName("Second Echelon")]
        public decimal SecondDecimalValue { get; set; }

        [DisplayName("Third Echelon")]
        public decimal ThirdDecimalValue { get; set; }

        [DisplayName("Fourth Echelon")]
        public decimal FourthDecimalValue { get; set; }

        public bool IsPaid { get; set; }

        public Reading reading { get; set; }

        public DateTime Date { get; set; }

        public User User { get; set; }
    }
}
