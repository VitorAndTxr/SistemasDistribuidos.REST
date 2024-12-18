
using SistemasDistribuidos.Ecommerce.Domain.ViewModel;

namespace SistemasDistribuidos.Ecommerce.Domain.Payload
{
    public class BuyRequestPayload
    {
        public string UserId { get; set; }
        public List<ProductViewModel> Items { get; set; }
    }
}

