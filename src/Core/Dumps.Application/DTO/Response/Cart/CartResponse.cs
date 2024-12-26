using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dumps.Application.DTO.Response.Products;
using Dumps.Domain.Entities;

namespace Dumps.Application.DTO.Response.Cart
{
    public class CartResponse
    {
        public Guid CartId { get; set; }
        public string UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public List<object> Items { get; set; }
    }
}
