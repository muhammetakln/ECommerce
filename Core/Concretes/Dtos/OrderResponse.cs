namespace Core.Concretes.DTOs
{
    public class OrderResponse
    {
        public bool Success { get; set; }
        public string? TransactionId { get; set; }
        public string? ErrorMessage { get; set; }
     
    }
}
