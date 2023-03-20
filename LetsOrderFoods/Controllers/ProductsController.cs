using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageUploader;
using LetsOrderFoods.Data;
using LetsOrderFoods.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LetsOrderFoods.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet("{id}")]
        public IActionResult Get()
        {
            return Ok(_context.Products);

        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Get(int id)
        {
            return Ok(_context.Products.Find(id));

        }

        [HttpGet("[action]/{Categoryid}")]
        public IActionResult ProductByCategoryId(int Categoryid)
        {
            var products = from p in _context.Products
                           where p.CategoryId == Categoryid
                           select new
                           {
                               Id = p.Id,
                               Name = p.Name,
                               Price = p.Price,
                               Description = p.Description,
                               CategoryId = p.CategoryId,
                               ImageUri = p.ImageUri
                           };

            return Ok(products);

        }

        [HttpGet("[action]")]
        public IActionResult PopularProducts()
        {
            var products = from p in _context.Products
                           where p.IsPopular == true
                           select new
                           {
                               Id = p.Id,
                               Name = p.Name,
                               Price = p.Price,
                               ImageUri = p.ImageUri
                           };

            return Ok(products);

        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Post([FromBody] Product product)
        {
            var stream = new MemoryStream(product.ImageArray);
            var guid = Guid.NewGuid().ToString();
            var fichier = $"{guid}.png";
            var dossier = "wwwroot";
            var mareponse = FilesHelper.UploadImage(stream, dossier, fichier);
            if (!mareponse)
            {
                return BadRequest();

            }
            else
            {
                product.ImageUri = fichier;
                _context.Products.Add(product);
                _context.SaveChanges();
                return StatusCode(StatusCodes.Status201Created);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Product product)
        {
            var monentity = _context.Products.Find(id);

            if (monentity == null)
            {
                return NotFound("Produit pas trouvée");

            }
            var stream = new MemoryStream(product.ImageArray);

            var guid = Guid.NewGuid().ToString();
            var fichier = $"{guid}.png";
            var dossier = "wwwroot";
            var mareponse = FilesHelper.UploadImage(stream, dossier, fichier);
            if (!mareponse)
            {
                return BadRequest();

            }
            else
            {
                monentity.CategoryId = product.CategoryId;
                monentity.Name = product.Name;
                monentity.Price = product.Price;
                monentity.Description = product.Description; 
                monentity.ImageUri = fichier;
                _context.SaveChanges();
                return Ok("Produit Mis à Jour avec Succès");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var monproduit = _context.Products.Find(id);

            if (monproduit == null)
            {
                return NotFound("Produit Pas Trouvée");

            }
            else
            {
                _context.Products.Remove(monproduit);
                _context.SaveChanges();
                return Ok("Products Supprimée Avec Succès");

            }
        }

    }
}

