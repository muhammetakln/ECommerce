using Core.Abstracts.Bases;

namespace Core.Concretes.Entities
{
    public class Brand : BaseEntity
    {
        public string Name { get; set; } = null!;
        // navigation properties
        public virtual ICollection<Product> Products { get; set; } = [];
    }

}
