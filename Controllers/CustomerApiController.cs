using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using StudentSpot.Models;
using StudentSpot.Services;

namespace StudentSpot.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerApiController : ControllerBase
    {
        private readonly myData _context;
        private readonly TokenService _tokenService;

        public CustomerApiController(myData context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        // ========================= REGISTER =========================
        [HttpPost("register")]
        public IActionResult Register(Customer customer)
        {
            if (_context.tbl_customer.Any(c => c.customer_email == customer.customer_email))
            {
                return BadRequest(new { message = "Email already exists" });
            }

            _context.tbl_customer.Add(customer);
            _context.SaveChanges();

            return Ok(new { message = "Customer registered successfully" });
        }

        // ========================= LOGIN =========================
        [HttpPost("login")]
        public IActionResult Login(string email, string password)
        {
            var customer = _context.tbl_customer
                .FirstOrDefault(c => c.customer_email == email
                                  && c.customer_password == password);

            if (customer == null)
                return Unauthorized(new { message = "Invalid credentials" });

            var token = _tokenService.GenerateToken(customer.customer_email);

            return Ok(new
            {
                token = token,
                customerId = customer.customer_id,
                name = customer.customer_name
            });
        }

        // ========================= GET ALL PRODUCTS =========================
        [HttpGet("products")]
        public IActionResult GetProducts()
        {
            var products = _context.tbl_product
                .Include(p => p.Category)
                .AsNoTracking()
                .ToList();

            return Ok(products);
        }

        // ========================= GET PRODUCT BY ID =========================
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

        // ========================= SEARCH =========================
        [HttpGet("search")]
        public IActionResult Search(string query)
        {
            var products = _context.tbl_product
                .Where(p => p.product_name.Contains(query)
                         || p.product_description.Contains(query))
                .ToList();

            return Ok(products);
        }

        // ========================= ADD TO CART =========================
        [Authorize(AuthenticationSchemes = "ApiScheme")]

        [HttpPost("add-to-cart")]
        public IActionResult AddToCart(int productId)
        {
            var email = User.Identity.Name;

            var customer = _context.tbl_customer
                .FirstOrDefault(c => c.customer_email == email);

            if (customer == null)
                return Unauthorized();

            var existingCart = _context.tbl_cart
                .FirstOrDefault(c => c.customer_id == customer.customer_id
                                  && c.prod_id == productId
                                  && c.status_id == 1);

            if (existingCart != null)
            {
                existingCart.product_quantity += 1;
            }
            else
            {
                var cart = new Cart
                {
                    customer_id = customer.customer_id,
                    prod_id = productId,
                    product_quantity = 1,
                    status_id = 1
                };

                _context.tbl_cart.Add(cart);
            }

            _context.SaveChanges();

            return Ok(new { message = "Added to cart" });
        }

        // ========================= VIEW CART =========================
        [Authorize(AuthenticationSchemes = "ApiScheme")]

        [HttpGet("cart")]
        public IActionResult GetCart()
        {
            var email = User.Identity.Name;

            var customer = _context.tbl_customer
                .FirstOrDefault(c => c.customer_email == email);

            var cart = _context.tbl_cart
                .Include(c => c.Products)
                .Where(c => c.customer_id == customer.customer_id
                         && c.status_id == 1)
                .ToList();

            return Ok(cart);
        }

        // ========================= WISHLIST =========================
        [Authorize]
        [HttpPost("wishlist/{productId}")]
        public IActionResult ToggleWishlist(int productId)
        {
            var email = User.Identity.Name;

            var customer = _context.tbl_customer
                .FirstOrDefault(c => c.customer_email == email);

            var existing = _context.tbl_wishlist
                .FirstOrDefault(w => w.customer_id == customer.customer_id
                                  && w.product_id == productId);

            if (existing == null)
            {
                _context.tbl_wishlist.Add(new Wishlist
                {
                    customer_id = customer.customer_id,
                    product_id = productId
                });
            }
            else
            {
                _context.tbl_wishlist.Remove(existing);
            }

            _context.SaveChanges();

            return Ok(new { message = "Wishlist updated" });
        }
    }
}
