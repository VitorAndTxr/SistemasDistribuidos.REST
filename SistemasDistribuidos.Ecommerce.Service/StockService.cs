using SistemasDistribuidos.Ecommerce.Domain.Entity;
using SistemasDistribuidos.Ecommerce.Service.Interface;

namespace SistemasDistribuidos.Ecommerce.Service
{
    public class StockService:IStockService
    {
        private List<Product> stock = InitStock();

        public StockService()
        {
            
        }

        public List<Product> GetProductsInStock()
        {
            return stock.Where(x => x.Stock > 0).ToList();
        }

        #region InitStock
        private static List<Product> InitStock()
        {
            return new List<Product>
            {
                new Product { 
                    Id = Guid.NewGuid(), 
                    Name = "Camisa",
                    Description = "Camisa de Algodão",
                    Image = "https://www.google.com.br",
                    Price = 50,
                    Stock = 10
                },
                new Product {
                    Id = Guid.NewGuid(),
                    Name = "Calça",
                    Description = "Camisa de Algodão",
                    Image = "https://www.google.com.br",
                    Price = 70,
                    Stock = 10
                },
                new Product {
                    Id = Guid.NewGuid(),
                    Name = "Blusa",
                    Description = "Camisa de Algodão",
                    Image = "https://www.google.com.br",
                    Price = 100,
                    Stock = 10
                },
                new Product {
                    Id = Guid.NewGuid(),
                    Name = "Jaqueta",
                    Description = "Camisa de Algodão",
                    Image = "https://www.google.com.br",
                    Price = 500,
                    Stock = 10
                },
                new Product {
                    Id = Guid.NewGuid(),
                    Name = "meia",
                    Description = "Camisa de Algodão",
                    Image = "https://www.google.com.br",
                    Price = 5,
                    Stock = 10
                }
            };
        }
        #endregion
    }
}
