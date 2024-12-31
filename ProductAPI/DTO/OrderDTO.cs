using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductAPI.DTO
{
    public class OrderDTO
    {
        public Guid OrderId { get; set; }
        public string ProductId { get; set; }

        public string UserId { get; set; }
        public int Quantity { get; set; }
    }
}