namespace SistemasDistribuidos.Ecommerce.Domain.ViewModel
{
    public class PaymentRequestViewModel
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public decimal Value { get; set; }
    }
}
