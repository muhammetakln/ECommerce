
namespace Core.Concretes.Entities
{
    public class  CartItem 
    {
        public int Id { get; set; }
        public int Quantity { get; set; }

        //Foreign Key
        public int CartId { get; set; }
        public int ProductId { get; set; }
        //Navigation Properties
        public virtual Cart? Cart { get; set; }
        public virtual Product? Product { get; set; }

        
    }
}
