using Core.Abstracts.Bases;

namespace Core.Concretes.Entities
{
    public class SubCategory : BaseEntity
    {
        public string Name { get; set; } = null!;
        // foreign keys
        public int CategoryId { get; set; }
        // navigation properties
        public virtual Category? Category { get; set; }
        public virtual ICollection<Product> Products { get; set; } = [];
    }

}
