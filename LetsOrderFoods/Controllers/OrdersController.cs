using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetsOrderFoods.Data;
using LetsOrderFoods.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LetsOrderFoods.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Post([FromBody] Order order)
        {
            order.IsCompleted = false;
            order.OrderDate = DateTime.Now;
            _context.Orders.Add(order);
            _context.SaveChanges();

            var monpanier = _context.CartItems.Where(crt => crt.CustomerId == order.UserId);
            foreach (var elt in monpanier)
            {
                var dtlcmd = new OrderDetail()
                {
                    Price = elt.Price,
                    OrderTotal = elt.TotalAmount,
                    Qty = elt.Qty,
                    ProductId = elt.ProductId,
                    OrderId = order.Id
                };
                _context.OrderDetails.Add(dtlcmd);
            }
            _context.SaveChanges();
            _context.CartItems.RemoveRange(monpanier);
            _context.SaveChanges();
            return Ok(new{OrderId = order.Id});
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("[action]")]
        public IActionResult PendingOrder()
        {
            var cmd = _context.Orders.Where(c => c.IsCompleted == false);
            return Ok(cmd);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("[action]")]
        public IActionResult Completed()
        {
            var cmd = _context.Orders.Where(c => c.IsCompleted == true);
            return Ok(cmd);
        }

        [HttpGet("[action]/{orderId}")]
        public IActionResult OrderDetails(int orderId)
        {
            var cmd = _context.Orders.Where(c => c.Id == orderId)
                .Include(h =>h.OrderDetails).ThenInclude(z=>z.Product);
            return Ok(cmd);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("[action]")]
        public IActionResult TotalOrder()
        {
            var orders = (from order in _context.Orders
                          where order.IsCompleted == false
                          select order.IsCompleted).Count();
            return Ok(new { PendingOrder = orders });
        }


        [HttpGet("[action]/{userId}")]
        public IActionResult OrdersByUser(int UserId)
        {
            var orders = _context.Orders.Where(x => x.UserId == UserId);

            return Ok(orders);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("[action]/{orderId}")]
        public IActionResult TargetOrderCompleted(int orderId, [FromBody] Order order)
        {
            var cmdfromDb = _context.Orders.Find(orderId);

            if(cmdfromDb == null)
            {
                return NotFound("Aucune Commande Trouvée");
            }
            else
            {
                cmdfromDb.IsCompleted = order.IsCompleted;
                _context.SaveChanges();
                return Ok("Votre Commande est Complete");
            }
        }
    }
}

