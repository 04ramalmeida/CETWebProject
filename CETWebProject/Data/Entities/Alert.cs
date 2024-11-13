namespace CETWebProject.Data.Entities
{
    public class Alert : IEntity
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public User User { get; set; }
    }
}
