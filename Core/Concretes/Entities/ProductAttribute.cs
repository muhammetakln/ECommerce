namespace Core.Concretes.Entities
{
    public class ProductAttribute
    {
        public int Id { get; set; }
        public string Key { get; set; } = null!;
        public string Value { get; set; } = null!;
        //Foreign key
        public int ProductId     { get; set; }
        // Navigation property
        public virtual Product? Products { get; set; }

    }
}
