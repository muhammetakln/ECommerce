using Core.Abstracts.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Concretes.Entities
{
    public class ProductReview : BaseEntity
    {
        public string Review { get; set; } = null!;
        public int Vote { get; set; }
        public string? AttachmentImage { get; set; }
        //Foreign Keys
        public string CustomerId { get; set; } = null!;
        public string ProductId { get; set; } = null!;
        // Navigation Properties
        public virtual Customer? Customer { get; set; }
        public virtual Product? Product { get; set; }


    }
}
