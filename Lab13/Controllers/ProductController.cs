﻿using Lab13.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProductController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Product
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        return await _context.Products.Where(p => !p.IsDeleted).ToListAsync();
    }

    // GET: api/Product/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null || product.IsDeleted)
        {
            return NotFound();
        }

        return product;
    }

    // PUT: api/Product/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProduct(int id, Product product)
    {
        if (id != product.IdProducts)
        {
            return BadRequest();
        }

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

    // POST: api/Product
    [HttpPost]
    public async Task<ActionResult<Product>> PostProduct(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetProduct", new { id = product.IdProducts }, product);
    }

    // DELETE: api/Product/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        product.IsDeleted = true;
        _context.Entry(product).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/Product/InsertProduct
    [HttpPost("InsertProduct")]
    public async Task<ActionResult<Product>> InsertProduct([FromBody] InsertProductRequest request)
    {
        var product = new Product
        {
            Name = request.Name,
            Price = request.Price,
            IsDeleted = false  // Assuming newly inserted products are not deleted by default
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetProduct", new { id = product.IdProducts }, product);
    }

    // DELETE: api/Product/DeleteProducts
    [HttpDelete("DeleteProducts")]
    public async Task<IActionResult> DeleteProducts([FromBody] List<Product> products)
    {
        foreach (var product in products)
        {
            var existingProduct = await _context.Products.FindAsync(product.IdProducts);
            if (existingProduct != null)
            {
                existingProduct.IsDeleted = true;
                _context.Entry(existingProduct).State = EntityState.Modified;
            }
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // PUT: api/Product/UpdatePrice
    [HttpPut("UpdatePrice")]
    public async Task<IActionResult> UpdatePrice([FromBody] UpdatePriceRequest request)
    {
        var product = await _context.Products.FindAsync(request.Id);
        if (product == null)
        {
            return NotFound();
        }

        product.Price = request.Price;

        _context.Entry(product).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ProductExists(int id)
    {
        return _context.Products.Any(e => e.IdProducts == id);
    }

    public class InsertProductRequest
    {
        public string Name { get; set; }
        public float Price { get; set; }
    }

    public class UpdatePriceRequest
    {
        public int Id { get; set; }
        public float Price { get; set; }
    }
}
