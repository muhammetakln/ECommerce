using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Core.Concretes.DTOs
{
    public record OrderRequestDto
    {

        public string OrderNumber { get; set; } = null!;
        public string Provider { get; set; } = "MockPay1";
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "TRY";              
        public string? Description { get; set; }
        public Dictionary<string, object>? MetaData { get; set; }

    }
}
