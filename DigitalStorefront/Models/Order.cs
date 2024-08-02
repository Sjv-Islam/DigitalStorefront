using System.ComponentModel.DataAnnotations;

namespace DigitalStorefront.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public string OrderCode { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime OrderDate { get; set; }
        public List<ProductDetail> ProductDetails { get; set; } = new List<ProductDetail>();
        public decimal TotalAmount { get; set; }
    }

    public class ProductDetail
    {
        public int ProductDetailId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }
    }

}


