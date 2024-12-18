
using SistemasDistribuidos.Ecommerce.Domain.ViewModel;

public class BuyRequestPayload
{
    public string UserId { get; set; }
    public List<ProductViewModel> Items { get; set; }
}