namespace SistemasDistribuidos.Ecommerce.Domain.Payload
{
    public class PaymentRequestPayload
    {
        public Guid BuyRequestId { get; set; }
        public string UserId { get; set; }
        public decimal TotalPrice { get; set; }
    }
}

