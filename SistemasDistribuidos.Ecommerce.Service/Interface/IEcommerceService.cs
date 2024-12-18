using SistemasDistribuidos.Ecommerce.Domain.Entity;
using SistemasDistribuidos.Ecommerce.Domain.ViewModel;

namespace SistemasDistribuidos.Ecommerce.Service.Interface
{
    public interface IEcommerceService {         
        List<Product> GetProductsInStock();
        void HandleBuyRequest(BuyRequestPayload payload);
        List<BuyRequestViewModel> GetUserBuyRequestList(string userId);
    }
    public interface ICartService { 
    
    }   
}
