using System;
namespace LetsOrderFoods.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public double Price { get; set; }

        public int Qty { get; set; }

        public double TotalAmount { get; set; }

        public int CustomerId { get; set; }
    }
}

