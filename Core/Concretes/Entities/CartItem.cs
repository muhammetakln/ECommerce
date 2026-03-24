namespace Core.Concretes.Entities
{
    public class  CartItem 
    {
        public int Id { get; set; }
        public int Quantity { get; set; }

        //Foreign Keys
        public string CartId { get; set; } = null!;
        public string ProductId { get; set; } = null!;
        //Navigation Properties
        public virtual Cart? Cart { get; set; }
        public virtual Product? Product { get; set; }


    }
}
