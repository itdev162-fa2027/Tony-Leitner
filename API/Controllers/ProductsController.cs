using Domain;
using Microsoft.AspNetCore.Mvc;
using Persistence;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly DataContext _context;

    public ProductsController(ILogger<ProductsController> logger, DataContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Product>> GetProducts()
    {
        var products = _context.Products.ToList();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public ActionResult<Product> GetProduct(int id)
    {
        var product = _context.Products.Find(id);
        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    public ActionResult<Product> CreateProduct(Product product)
    {
        product.CreatedDate = DateTime.Now;
        product.LastUpdatedDate = DateTime.Now;

        _context.Products.Add(product);
        var success = _context.SaveChanges() > 0;
        if (success)
        {
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        return BadRequest("Failed to create product");
    }

    [HttpPut("{id}")]
    public ActionResult<Product> UpdateProduct(int id, Product product)
    {
        var existingProduct = _context.Products.Find(id);
        if (existingProduct == null)
        {
            return NotFound();
        }

        existingProduct.Name = product.Name;
        existingProduct.Description = product.Description;
        existingProduct.Price = product.Price;
        existingProduct.IsOnSale = product.IsOnSale;
        existingProduct.SalePrice = product.SalePrice;
        existingProduct.CurrentStock = product.CurrentStock;
        existingProduct.ImageUrl = product.ImageUrl;
        existingProduct.LastUpdatedDate = DateTime.Now;

        var success = _context.SaveChanges() > 0;
        if (success)
        {
            return Ok(existingProduct);
        }

        return BadRequest("Failed to update product");
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteProduct(int id)
    {
        var product = _context.Products.Find(id);
        if (product == null)
        {
            return NotFound();
        }

        _context.Products.Remove(product);
        var success = _context.SaveChanges() > 0;
        if (success)
        {
            return NoContent();
        }

        return BadRequest("Failed to delete product");
    }

    [HttpGet("search")]
    public ActionResult<IEnumerable<Product>> SearchProducts(
        [FromQuery] string? name = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] bool? isOnSale = null,
        [FromQuery] bool? inStock = null,
        [FromQuery] string sortBy = "name",
        [FromQuery] string sortOrder = "asc")
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(product => product.Name.ToLower().Contains(name.ToLower()));
        }

        if (minPrice.HasValue)
        {
            query = query.Where(product => product.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(product => product.Price <= maxPrice.Value);
        }

        if (isOnSale.HasValue)
        {
            query = query.Where(product => product.IsOnSale == isOnSale.Value);
        }

        if (inStock.HasValue && inStock.Value)
        {
            query = query.Where(product => product.CurrentStock > 0);
        }

        var products = query.ToList();
        var descending = sortOrder.ToLower() == "desc";

        products = sortBy.ToLower() switch
        {
            "price" => descending
                ? products.OrderByDescending(product => product.Price).ToList()
                : products.OrderBy(product => product.Price).ToList(),
            "created" => descending
                ? products.OrderByDescending(product => product.CreatedDate).ToList()
                : products.OrderBy(product => product.CreatedDate).ToList(),
            "stock" => descending
                ? products.OrderByDescending(product => product.CurrentStock).ToList()
                : products.OrderBy(product => product.CurrentStock).ToList(),
            _ => descending
                ? products.OrderByDescending(product => product.Name).ToList()
                : products.OrderBy(product => product.Name).ToList()
        };

        return Ok(products);
    }
}
