using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentSpot.Models;

namespace StudentSpot.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerApiController : ControllerBase
    {
        private readonly myData _context;

        public CustomerApiController(myData context)
        {
            _context = context;
        }

        // ================= GET ALL PRODUCTS =================
        [HttpGet("products")]
        public IActionResult GetProducts()
        {
            var products = _context.tbl_product
                .Include(p => p.Category)
                .AsNoTracking()
                .ToList();

            return Ok(products);
        }

        // ================= GET PRODUCT BY ID =================
        [HttpGet("product/{id}")]
        public IActionResult GetProduct(int id)
        {
            var product = _context.tbl_product
                .Include(p => p.Category)
                .FirstOrDefault(p => p.product_id == id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // ================= SEARCH =================
        [HttpGet("search")]
        public IActionResult Search(string query)
        {
            var products = _context.tbl_product
                .Where(p => p.product_name.Contains(query) ||
                            p.product_description.Contains(query))
                .ToList();

            return Ok(products);
        }

        // ================= GET ALL CATEGORIES =================
        [HttpGet("categories")]
        public IActionResult GetCategories()
        {
            var categories = _context.tbl_category
                .AsNoTracking()
                .ToList();

            return Ok(categories);
        }
    }
}