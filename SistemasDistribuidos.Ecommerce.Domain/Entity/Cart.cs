namespace SistemasDistribuidos.Ecommerce.Domain.Entity
{
    public class Cart
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public List<Product> Products { get; set; } = [];

        public decimal TotalPrice => Products.Sum(p => p.Price);
    }
}
