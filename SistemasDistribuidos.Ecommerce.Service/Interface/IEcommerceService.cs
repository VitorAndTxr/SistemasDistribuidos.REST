using SistemasDistribuidos.Ecommerce.Domain.Entity;
using SistemasDistribuidos.Ecommerce.Domain.Payload;
using SistemasDistribuidos.Ecommerce.Domain.ViewModel;

namespace SistemasDistribuidos.Ecommerce.Service.Interface
{
    public interface IEcommerceService {         
        List<Product> GetProductsInStock();
        void HandleCreateBuyRequest(BuyRequestPayload payload);
        List<BuyRequestViewModel> GetUserBuyRequestList(string userId);
        void HandlePaymentRequestResponse(BuyRequestViewModel payload);
    } 
}
