using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScisaApi.Data;
using ScisaApi.DTOs;
using ScisaApi.Models;

namespace ScisaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly DataContext _context;

        public CategoriesController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RetrieveCategory>>> GetCategories()
        {
            return await _context.Categories.Select(c=> new RetrieveCategory
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Products = (List<RetrieveCategoryProducts>)c.Products.Select(p => new RetrieveCategoryProducts
                {
                    Id= p.Id,
                    Name = p.Name,
                    Description = p.Description,
                }),
            }).ToListAsync();
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RetrieveCategory>> GetCategory(int id)
        {
            var category = await _context.Categories.Select(c => new RetrieveCategory
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Products = (List<RetrieveCategoryProducts>)c.Products.Select(p => new RetrieveCategoryProducts
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                }),
            }).FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, CreateCategory categoryDto)
        {
            var categoryEdit = await _context.Categories.FindAsync(id);

            if (categoryEdit == null)
            {
                return NotFound();
            }

            if (categoryEdit.Name != categoryDto.Name)
            {
                var existingCategory = await _context.Categories.AnyAsync(c => c.Name == categoryDto.Name);

                if (existingCategory)
                {
                    return Conflict(new { message = "El nombre de la categoría ya existe." });
                }

                categoryEdit.Name = categoryDto.Name;
                categoryEdit.Description = categoryDto.Description;

                _context.Entry(categoryEdit).State = EntityState.Modified;

            }
            else
            {
                categoryEdit.Description = categoryDto.Description;
                _context.Entry(categoryEdit).State = EntityState.Modified;
            }
            

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(CreateCategory categoryDto)
        {
            var existingCategory = await _context.Categories.AnyAsync(c => c.Name == categoryDto.Name);

            if (existingCategory)
            {
                return Conflict(new { message = "El nombre de la categoría ya existe." });
            }

            var category = new Category { Name = categoryDto.Name, Description= categoryDto.Description };


            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.Include(c=>c.Products).FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            if (category.Products.Any())
            {
                return Conflict(new { message = "No se puede eliminar la categoría porque tiene productos asignados." });
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
