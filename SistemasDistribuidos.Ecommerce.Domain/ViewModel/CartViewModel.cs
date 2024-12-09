namespace SistemasDistribuidos.Ecommerce.Domain.ViewModel
{
    public class CartViewModel
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public List<ProductViewModel> Products { get; set; } = [];

        public decimal TotalPrice => Products.Sum(p => p.Price);
    }
}
