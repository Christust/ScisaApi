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
    public class ProductsController : ControllerBase
    {
        private readonly DataContext _context;

        public ProductsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.Include(p=>p.Categories).ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.Include(p=>p.Categories).FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, ProductCreateDto productDto)
        {
            var product = await _context.Products
            .Include(p => p.Categories)
            .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            if (productDto.CategoryIds == null || !productDto.CategoryIds.Any())
            {
                return BadRequest(new { message = "El producto debe tener al menos una categoría." });
            }

            var categories = await _context.Categories
            .Where(c => productDto.CategoryIds.Contains(c.Id))
            .ToListAsync();

            if (categories.Count == 0)
            {
                return BadRequest(new { message = "Las categorías especificadas no existen." });
            }

            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.Categories = categories;


            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(ProductCreateDto productDto)
        {
            var categories = await _context.Categories
            .Where(c => productDto.CategoryIds.Contains(c.Id))
            .ToListAsync();

            if (categories.Count == 0)
            {
                return BadRequest(new { message = "Debe existir al menos una cateoria." });
            }

            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
            };

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            foreach (var category in categories)
            {
                product.Categories.Add(category);
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
