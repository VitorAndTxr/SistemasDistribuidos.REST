using SistemasDistribuidos.Ecommerce.Domain.Entity;

namespace SistemasDistribuidos.Ecommerce.Service.Interface
{
    public interface IEcomerceService {         
        List<Product> GetProductsInStock();
    }
    public interface ICartService { 
    
    }   
}
