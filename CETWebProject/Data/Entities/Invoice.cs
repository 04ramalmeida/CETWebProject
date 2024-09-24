using System;

namespace CETWebProject.Data.Entities
{
    public class Invoice : IEntity
    {
        public int Id { get; set; }

        public decimal Value { get; set; }

        public DateTime Date { get; set; }

        public User User { get; set; }
    }
}
