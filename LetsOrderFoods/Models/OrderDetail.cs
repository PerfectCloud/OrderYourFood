using System;
namespace LetsOrderFoods.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }

        public Order Order { get; set; }

        public int OrderId { get; set; }

        public double OrderTotal { get; set; }

        public int Qty { get; set; }

        public double Price { get; set; }
    }
}

