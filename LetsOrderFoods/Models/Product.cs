using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LetsOrderFoods.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageUri { get; set; }

        [NotMapped]

        public byte[] ImageArray { get; set; }

        public double Price { get; set; }

        public bool IsPopular { get; set; }

        public int CategoryId { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }

        public ICollection<Cart> CartItems { get; set; }
    }
}

