using Repositories.Entities;
using Repositories.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models.OrderModels
{
    public class GetAllOrdersModel
    {
        public Guid OrderID { get; set; }
        public Guid AccountID { get; set; }
        public int PaymentMethod { get; set; }
        public string BillingAddress { get; set; }
        public int Status { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime CreateDate { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderCartItemModel> OrderCartItemModels { get; set; }

    }
    public class OrderCartItemModel
    {
        public Guid ProductSizeID { get; set; }
        public string SizeName { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int OrderStatus { get; set; } 
    }
}
