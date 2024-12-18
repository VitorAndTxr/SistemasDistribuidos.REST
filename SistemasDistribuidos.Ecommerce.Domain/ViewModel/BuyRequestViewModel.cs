﻿using SistemasDistribuidos.Ecommerce.Domain.Enum;

namespace SistemasDistribuidos.Ecommerce.Domain.ViewModel
{
    public class BuyRequestViewModel
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public BuyRequestStatusEnum Status { get; set; }
        public List<ProductViewModel> Products { get; set; } = [];

        public decimal TotalPrice => Products.Sum(p => p.Price);
    }
}