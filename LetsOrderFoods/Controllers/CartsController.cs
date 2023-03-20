using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetsOrderFoods.Data;
using Microsoft.AspNetCore.Mvc;
using LetsOrderFoods.Models;
using System.Data;
using Microsoft.AspNetCore.Http;

namespace LetsOrderFoods.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CartsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{userId}")]
        public IActionResult Get(int userId)
        {
            var user = _context.CartItems.Where(ct => ct.CustomerId == userId);
            if(user == null)
            {
                return NotFound();
            }

            var cartElts = from ctelt in _context.CartItems.Where(
                c => c.CustomerId == userId)
                           join
                           b in _context.Products on ctelt.ProductId equals b.Id
                           select new
                           {
                               Id = ctelt.Id,
                               Price = ctelt.Price,
                               TotalAmount = ctelt.TotalAmount,
                               Qty = ctelt.Qty,
                               Name = b.Name
                           };
            return Ok(cartElts);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Cart carts)
        {
            var panier = _context.CartItems.FirstOrDefault(n => n.ProductId == carts.ProductId &&
            n.CustomerId == carts.CustomerId);
            if(panier != null)
            {
                panier.Qty = carts.Qty;
                panier.TotalAmount = carts.TotalAmount;

            }
            else
            {
                var PanierShopping = new Cart()
                {
                    CustomerId = panier.CustomerId,
                    Price = panier.Price,
                    ProductId = panier.ProductId,
                    Qty = panier.Qty,
                    TotalAmount = panier.TotalAmount,
                };
                _context.CartItems.Add(PanierShopping);
            }
            _context.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpGet("[action]/{userId}")]
        public IActionResult TotalElements(int userId)
        {
            var PanierElement = (from panier in _context.CartItems
                                 where panier.CustomerId == userId
                                 select panier.Qty).Sum();
            return Ok(new { TotalElements = PanierElement });

        }

        [HttpGet("[action]/{userId}")]
        public IActionResult TotalAmount(int userId)
        {
            var MontantTotal = (from panier in _context.CartItems
                                 where panier.CustomerId == userId
                                 select panier.TotalAmount).Sum();
            return Ok(new { TotalAmount = MontantTotal });

        }

        [HttpGet("{userId}")]
        public IActionResult Delete(int userId)
        {
            var Panier = _context.CartItems.Where(p => p.CustomerId == userId);
            _context.CartItems.RemoveRange(Panier);
            _context.SaveChanges();
            return Ok();

        }


    }
 }

