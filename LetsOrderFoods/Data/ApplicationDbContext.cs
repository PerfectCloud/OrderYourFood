using System;
using LetsOrderFoods.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LetsOrderFoods.Data
{
        public class ApplicationDbContext:DbContext
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
                base(options)
            {

            }

            public DbSet<User> Users { get; set; }
            public DbSet<Product> Products { get; set; }
            public DbSet<Category> Categories { get; set; }
            public DbSet<Order> Orders { get; set; }
            public DbSet<OrderDetail> OrderDetails { get; set; }
            public DbSet<Cart> CartItems { get; set; }
        }
}

