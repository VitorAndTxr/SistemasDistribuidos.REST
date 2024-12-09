using SistemasDistribuidos.Ecommerce.Domain.Entity;

namespace SistemasDistribuidos.Ecommerce.Service.Interface
{
    public interface IStockService
    {
        List<Product> GetProductsInStock();
    }
}
