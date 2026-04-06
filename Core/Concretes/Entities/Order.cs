using Core.Abstracts.Bases;
using Core.Concretes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Core.Concretes.Entities
{
    public class Order:BaseEntity
    {
        public string CustomerId { get; set; } = null!;
        public virtual Customer? Customer    { get; set; }
        public int CartId { get; set; }//foreign key değil:Tabloları bir birine bağlamaz
        public ICollection<OrderITem> Items { get; set; } = [];
        public OrderStatus Status { get; set; }
    }
}
