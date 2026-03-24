using Core.Abstracts.Bases;

namespace Core.Concretes.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = null!;
        // navigation properties
        public virtual ICollection<SubCategory> SubCategories { get; set; } = [];
    }

}
