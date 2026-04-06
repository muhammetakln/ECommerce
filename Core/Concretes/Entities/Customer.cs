using Microsoft.AspNetCore.Identity;

namespace Core.Concretes.Entities
{
    public class Customer:IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string City { get; set; } = null!;
        public string District { get; set; } = null!;
        

        // Navigation property 
        public virtual ICollection<Cart> ShoppingCarts { get; set; } = [];
        public virtual ICollection<ProductReview> ProductReviews { get; set; } = [];

        public virtual ICollection<Order> Orders { get; set; } = [];

    }
}
