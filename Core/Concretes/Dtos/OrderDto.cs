using Core.Concretes.Entities;
using Core.Concretes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Concretes.Dtos
{
    public class OrderDto
    {
        public int Id { get; set; } 
        public virtual IEnumerable<OrderItemDto> Items { get; set; } = [];
        public OrderStatus Status { get; set; }
    }
    public class OrderItemDto
    {

       
        public int Quantity { get; set; }
        public string ProductName { get; set; } = null!;

        public int ProductId { get; set; }
        public string? ProductImage { get; set; }
        public decimal ListPrice { get; set; }
        public decimal DiscountValue { get; set; }
    }
}
