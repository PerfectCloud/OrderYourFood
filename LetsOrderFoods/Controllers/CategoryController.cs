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
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Get()
        {
            var lescategories = from cat in _context.Categories
                                select new
                                {
                                    Id = cat.Id,
                                    Name = cat.Name,
                                    ImageUri = cat.ImageUri
                                };
            return Ok(lescategories);
        }


        [Authorize(Roles ="Admin")]
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var lscategories = (from ctg in _context.Categories
                                where ctg.Id == id
                                select new
                                {
                                    Id = ctg.Id,
                                    Name = ctg.Name,
                                    ImageUri = ctg.ImageUri
                                }).FirstOrDefault();
            return Ok(lscategories);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Post([FromBody] Category category)
        {
            var stream = new MemoryStream(category.ImageArray);

            var guid = Guid.NewGuid().ToString();
            var fichier = $"{guid}.png";
            var dossier = "wwwroot";

            var mareponse = FilesHelper.UploadImage(stream, dossier, fichier);
            if(!mareponse)
            {
                return BadRequest();

            }
            else
            {
                category.ImageUri = fichier;
                _context.Categories.Add(category);
                _context.SaveChanges();
                return StatusCode(StatusCodes.Status201Created);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Category category)
        {
            var monentity = _context.Categories.Find(id);

            if(monentity == null)
            {
                return NotFound("Categorie pas trouvée");

            }
            var stream = new MemoryStream(category.ImageArray);

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
                monentity.Name = category.Name;
                monentity.ImageUri = fichier;
                _context.SaveChanges();
                return Ok("Categore Mise à Jour avec Succès");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var macategorie = _context.Categories.Find(id);

            if(macategorie == null)
            {
                return NotFound("Categorie Pas Trouvée");

            }
            else
            {
                _context.Categories.Remove(macategorie);
                _context.SaveChanges();
                return Ok("Categorie Supprimée Avec Succès");
            }
        }
    }
}