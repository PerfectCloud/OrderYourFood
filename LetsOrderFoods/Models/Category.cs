using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LetsOrderFoods.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImageUri { get; set; }

        [NotMapped]

        public byte[] ImageArray { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}

