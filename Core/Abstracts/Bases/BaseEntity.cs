namespace Core.Abstracts.Bases
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }        
        public bool Deleted { get; set; }
        public bool Active { get; set; }

    }
}
