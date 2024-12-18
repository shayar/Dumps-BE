using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumps.Application.DTO.Response.Cart
{
    public class CartResponse
    {
        public Guid CartId { get; set; }
        public string UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public List<CartItemResponse> CartItems { get; set; }
    }

    
    public class CartItemResponse
    {
        public Guid CartItemId { get; set; }
        public Guid? ProductId { get; set; }
        public string ProductTitle { get; set; }
        public decimal? ProductPrice { get; set; }
        public Guid? BundleId { get; set; }
        public string BundleTitle { get; set; }
        public decimal ItemPrice { get; set; }
    }
}
