using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentSpot.Models;

using StudentSpot.Services;

namespace StudentSpot.Controllers
{
    public class Customercontroller : Controller
    {
        private readonly myData _context;
    private readonly HttpClient _httpClient;

    public Customercontroller(myData context, IHttpClientFactory factory)
    {
        _context = context;
        _httpClient = factory.CreateClient();
    }

        [ResponseCache(Duration = 60)]
        public IActionResult Index()
        {

            List<Category> category = _context.tbl_category.AsNoTracking().ToList();
            ViewData["category"] = category;


            // Get only 4 latest products (e.g. New Arrivals)
            List<Product> product = _context.tbl_product
                .OrderByDescending(p => p.product_id) // or .CreatedDate if available
                .AsNoTracking().Take(4)
                .ToList();
            ViewData["product"] = product;



            // Get only 4 latest products (e.g. New Arrivals)
            List<Bestblog> latestBlogs = _context.tbl_bestblog
                   .OrderByDescending(b => b.blog_id)
                   .Take(4)
                   .ToList();
            ViewData["bestblogs"] = latestBlogs;



            ViewBag.checksession = HttpContext.Session.GetString("customerSession");
        


            var products = _context.tbl_product
                .Include(p => p.Category)
                .AsNoTracking()
                .Take(150)
                .ToList();

            // category wise data
            ViewData["BottleProducts"] = products
                .Where(p => p.Category.category_name == "BestSellers")
                .ToList();

            ViewData["LaptopProducts"] = products
                .Where(p => p.Category.category_name == "wirelessProducts")
                .ToList();
            ViewData["TechnologyProducts"] = products
        .Where(p => p.Category.category_name == "Technology")
        .ToList();


            ViewData["ContinueShoppingDeals"] = products
                .Where(p => p.Category.category_name == "shoppingdeals")
                .ToList();

            ViewData["DiscoverLuxuryBrands"] = products
                .Where(p => p.Category.category_name == "luxurybrands")
                .ToList();

            ViewData["LuxuryGifts"] = products
                .Where(p => p.Category.category_name == "luxurygifts")
                .ToList();

            ViewData["AmazonDevices"] = products
                .Where(p => p.Category.category_name == "Devices")
                .ToList();


            // PCs
            ViewData["AmazonPCs"] = products
                .Where(p => p.Category.category_name == "PC")
                .ToList();

            // Accessories
            ViewData["AmazonAccessories"] = products
                .Where(p => p.Category.category_name == "Gear")
                .ToList();

            // Bags
            ViewData["AmazonBags"] = products
          .Where(p => p.Category.category_name.ToLower() == "purchased")
          .ToList();

            return View();


        }
        //======================================================== category dropdron==========================
        public IActionResult FetchListCategory()
        {
            List<Category> category = _context.tbl_category.AsNoTracking().ToList();
            ViewData["category"] = category;
            var categories = _context.tbl_category.AsNoTracking().ToList();
            return PartialView("fetchlistcategory", categories);
        }
        //======================================login================================================================

        public IActionResult customerLogin()

        {
            List<Category> category = _context.tbl_category.AsNoTracking().ToList();
            ViewData["category"] = category;
            return View();
        }
        [HttpPost]
        public IActionResult customerLogin(string customerEmail, string customerPassword)
        {
            var customer = _context.tbl_customer.FirstOrDefault(c => c.customer_email == customerEmail);

            if (customer != null && customer.customer_password == customerPassword)
            {
                // ✅ Session set
                HttpContext.Session.SetString("customer_email", customer.customer_email);
                HttpContext.Session.SetString("customerSession", customer.customer_id.ToString());

                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.message = "Incorrect Username or Password";
                ViewData["category"] = _context.tbl_category.ToList();
                return View();
            }
        }

        public IActionResult Logout()
        {

            HttpContext.Session.Remove("customerSession");
            return RedirectToAction("Index");
        }

        public IActionResult customerRegister()
        {
            ViewData["category"] = _context.tbl_category.AsNoTracking().ToList();
            return View();
        }
        [HttpPost]
        public IActionResult CustomerRegister(Customer customer)
        {
            _context.tbl_customer.Add(customer);
            _context.SaveChanges();
            return RedirectToAction("customerLogin");
        }

        //=====================================================================CUSTOMER Profile=================================
        public IActionResult customerProfile()
        {
            List<Category> category = _context.tbl_category.AsNoTracking().ToList();
            ViewData["category"] = category;
            var customerIdString = HttpContext.Session.GetString("customerSession");

            if (string.IsNullOrEmpty(customerIdString))
            {
                // If user is not logged in, redirect to login page
                return RedirectToAction("customerLogin");
            }

            int customerId = int.Parse(customerIdString);
            var customer = _context.tbl_customer.FirstOrDefault(c => c.customer_id == customerId);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        public IActionResult update_customer_profile()
        {
            var customerIdString = HttpContext.Session.GetString("customerSession");

            if (string.IsNullOrEmpty(customerIdString))
            {
                return RedirectToAction("customerLogin");
            }

            int customerId = int.Parse(customerIdString);
            var customer = _context.tbl_customer.FirstOrDefault(c => c.customer_id == customerId);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        [HttpPost]
      
        [ValidateAntiForgeryToken]
        public IActionResult update_customer_profile(Customer updatedCustomer, IFormFile? customerImage)
        {
            var customerIdString = HttpContext.Session.GetString("customerSession");

            if (string.IsNullOrEmpty(customerIdString))
            {
                return RedirectToAction("customerLogin");
            }

            int customerId = int.Parse(customerIdString);
            var existingCustomer = _context.tbl_customer.FirstOrDefault(c => c.customer_id == customerId);

            if (existingCustomer == null)
            {
                return NotFound();
            }

            // Update basic fields
            existingCustomer.customer_name = updatedCustomer.customer_name;
            existingCustomer.customer_email = updatedCustomer.customer_email;
            existingCustomer.customer_phone = updatedCustomer.customer_phone;
            existingCustomer.customer_adress = updatedCustomer.customer_adress;
            existingCustomer.customer_gender = updatedCustomer.customer_gender;

            // Handle image upload (optional)
            if (customerImage != null && customerImage.Length > 0)
            {
                string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/customer_image");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                // Unique file name to avoid overwriting
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(customerImage.FileName);
                string filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    customerImage.CopyTo(stream);
                }

                // Save image path in DB
                existingCustomer.customer_image = "/customer_image/" + uniqueFileName;
            }

            _context.SaveChanges();

            return RedirectToAction("customerProfile");
        }


        // ========================
        [HttpGet]
        public IActionResult Search(string query)
        {
            List<Category> category = _context.tbl_category.AsNoTracking().ToList();
            ViewData["category"] = category;

            var products = _context.tbl_product.AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                products = products.Where(p => p.product_name.Contains(query)
                                            || p.product_description.Contains(query));
            }

            ViewData["product"] = products.OrderByDescending(p => p.product_id).ToList();
            ViewBag.SearchQuery = query;

            return View("fetchProducts_all"); // same view reuse karenge
        }

        //}
        //=============================================================
      
         public IActionResult fetchProducts_all(string? category)
        {
            ViewData["category"] = _context.tbl_category.AsNoTracking().ToList(); 
            var products = _context.tbl_product.Include(p => p.Category).AsQueryable(); 
            if (!string.IsNullOrEmpty(category)) 
            { products = products.Where(p => p.Category.category_name == category); }

            ViewData["product"] = products.OrderByDescending(p => p.product_id).ToList();
            ViewBag.SelectedCategory = category; 
            return View();
        }

        public IActionResult pruductsDitail(int id)
        {
            List<Category> category = _context.tbl_category.AsNoTracking().ToList();
            ViewData["category"] = category;

            var products = _context.tbl_product.Where(p => p.product_id == id).ToList();
            return View(products);

        }
        //========================================

        public IActionResult fetchBlog()
        {

            List<Category> category = _context.tbl_category.AsNoTracking().ToList();
            ViewData["category"] = category;

            List<Bestblog> latestBlogs = _context.tbl_bestblog
                  .OrderByDescending(b => b.blog_id)
                  //.Take()
                  .ToList();
            ViewData["bestblogs"] = latestBlogs;
            return View();

        }
        public IActionResult blogDitail(int id)
        {
            List<Category> category = _context.tbl_category.ToList();
            ViewData["category"] = category;

            var blog = _context.tbl_bestblog.Where(b => b.blog_id == id).ToList();
            return View(blog);

        }
        //==========================================================================
        public IActionResult About()
        {
            List<Category> category = _context.tbl_category.AsNoTracking().ToList();
            ViewData["category"] = category;
            return View();
        }
        //=============================================================================



        public IActionResult feedback()
        {
            List<Category> category = _context.tbl_category.AsNoTracking().ToList();
            ViewData["category"] = category;
            return View();
        }
        [HttpPost]
        public IActionResult feedback(Feedback feedback)
        {
            _context.tbl_feedback.Add(feedback);
            _context.SaveChanges();
            return RedirectToAction("feedback");
        }



















        //======================================
        //is ka kaam hi data ko admin ki trah lay ky jana jb clik hu add batton pr ===============================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart(int prod_id)
        {
            if (!HttpContext.Session.Keys.Contains("customerSession"))
            {
                return RedirectToAction("customerLogin");
            }

            int customerId = int.Parse(HttpContext.Session.GetString("customerSession"));

            var existingCart = _context.tbl_cart
                .FirstOrDefault(c => c.customer_id == customerId
                                  && c.prod_id == prod_id
                                  && c.status_id == 1);

            if (existingCart != null)
            {
                existingCart.product_quantity += 1;
            }
            else
            {
                _context.tbl_cart.Add(new Cart
                {
                    customer_id = customerId,
                    prod_id = prod_id,
                    product_quantity = 1,
                    status_id = 1
                });
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]

        //public IActionResult AddToCart(int prod_id)

        //{
        //https://localhost:7344/Customer/AddToCart/34

        //}
        //    var email = HttpContext.Session.GetString("customer_email");

        //    if (string.IsNullOrEmpty(email))
        //    {
        //        return RedirectToAction("customerLogin");
        //    }


        //    var customer = _context.tbl_customer.FirstOrDefault(c => c.customer_email == email);
        //    if (customer == null)
        //        return RedirectToAction("customerLogin");

        //    var existingCart = _context.tbl_cart
        //        .FirstOrDefault(c => c.customer_id == customer.customer_id
        //                          && c.prod_id == prod_id
        //                          && c.status_id == 1);

        //    if (existingCart != null)
        //    {
        //        existingCart.product_quantity += 1;
        //    }
        //    else
        //    {
        //        _context.tbl_cart.Add(new Cart
        //        {
        //            customer_id = customer.customer_id,
        //            prod_id = prod_id,
        //            product_quantity = 1,
        //            status_id = 1
        //        });
        //    }

        //    _context.SaveChanges();

        //    return RedirectToAction("Index"); // یا جہاں آپ cart/show کرنا چاہتے ہیں
        //}



        //========================================================================


        // is ka kaam hi batton pr 1234 add krna 

        public JsonResult GetCartItemCount()
        {
            try
            {
                string customerId = HttpContext.Session.GetString("customerSession");

                if (!string.IsNullOrEmpty(customerId))
                {
                    int custId = int.Parse(customerId);

                    int count = _context.tbl_cart
                        .Where(c => c.customer_id == custId && c.status_id == 1)
                        .Sum(c => c.product_quantity);

                    return Json(count);
                }

                return Json(0);
            }
            catch
            {
                return Json(0);
            }
        }
        //===============================================
        public IActionResult fetchCart()
        {
            // Load all categories to display in navbar (optional)
            List<Category> category = _context.tbl_category.AsNoTracking().ToList();
            ViewData["category"] = category;

            // Get customer ID from session
            string customerId = HttpContext.Session.GetString("customerSession");

            if (!string.IsNullOrEmpty(customerId))
            {
                int custId = int.Parse(customerId);

                // Fetch cart items for this customer, only those with status_id = 1 (active)
                var cart = _context.tbl_cart
                    .Where(c => c.customer_id == custId && c.status_id == 1)
                    .Include(c => c.Products)  // ✅ Products load ho rahe hain
                    .ToList();

                return View(cart);
            }
            else
            {
                return RedirectToAction("customerLogin");
            }
        }
        //=============================================================
        public IActionResult RemoveFromCart(int id)
        {
            // Find the cart item by ID
            var cartItem = _context.tbl_cart.FirstOrDefault(c => c.cart_id == id);

            if (cartItem != null)
            {
                _context.tbl_cart.Remove(cartItem);
                _context.SaveChanges();
                TempData["Message"] = "Item removed from cart.";
            }
            else
            {
                TempData["Message"] = "Cart item not found.";
            }

            return RedirectToAction("fetchCart");
        }

        //=============================================================
        public IActionResult Checkout(int id)
        {
            ViewData["category"] = _context.tbl_category.AsNoTracking().ToList();  // MUST

            var cartItem = _context.tbl_cart
                            .Include(c => c.Products)
                            .FirstOrDefault(c => c.cart_id == id);

            if (cartItem == null)
                return NotFound();

            decimal price = 0;
            int quantity = cartItem.product_quantity;

            decimal.TryParse(cartItem.Products.product_price, out price);

            ViewBag.TotalAmount = price * quantity;

            return View(cartItem);
        }

        //======================================================================= 
        // order rplce krna hi 


        [HttpPost]
        public IActionResult PlaceOrder()
        {
            string islogin = HttpContext.Session.GetString("customerSession");

            if (string.IsNullOrEmpty(islogin))
                return RedirectToAction("customerLogin");

            int customerId = int.Parse(islogin);

            var customerCartItems = _context.tbl_cart
                .Where(c => c.customer_id == customerId && c.status_id == 1)
                .ToList();

            if (!customerCartItems.Any())
            {
                TempData["Message"] = "Your cart is empty.";
                return RedirectToAction("fetchCart");
            }

            foreach (var cart in customerCartItems)
            {
                // Create BayNow order
                var newOrder = new BayNow
                {
                    carts_id = cart.cart_id,
                    Bay_status_id = 0, // Pending
                    CreatedAt = DateTime.Now
                };

                _context.tbl_baynow.Add(newOrder);

                // Optional: Update cart status to processed (status_id = 2)
                cart.status_id = 2;
            }

            _context.SaveChanges();

            TempData["Message"] = "Order placed successfully!";
            return RedirectToAction("fetchCart");
        }
        //================================================================
        // is kam wishlist ka hi 
        // ✅ Add or Remove from Wishlist (Toggle)
        [HttpPost]
        public IActionResult AddToWishlist(int prod_id)
        {
            ViewData["category"] = _context.tbl_category.AsNoTracking().ToList();

            // Session se customer ID nikalna
            string? customerSession = HttpContext.Session.GetString("customerSession");

            if (string.IsNullOrEmpty(customerSession))
            {
                // agar login nahi hai to login page par bhej do
                return RedirectToAction("customerLogin", "customer");
            }

            int customerId = int.Parse(customerSession);

            // check karo product already wishlist me hai ya nahi
            var existing = _context.tbl_wishlist
                .FirstOrDefault(w => w.customer_id == customerId && w.product_id == prod_id);

            if (existing == null)
            {
                // add new wishlist entry
                var wishlist = new Wishlist
                {
                    customer_id = customerId,
                    product_id = prod_id
                };
                _context.tbl_wishlist.Add(wishlist);
            }
            else
            {
                // agar already wishlist me hai to remove kar do (toggle)
                _context.tbl_wishlist.Remove(existing);
            }

            _context.SaveChanges();

            // same page par redirect karo (products list par)
            return RedirectToAction("fetchProducts_all", "customer");
        }

        // ✅ Show Wishlist Products
        public IActionResult Wishlist()
        {
            string? customerSession = HttpContext.Session.GetString("customerSession");

            if (string.IsNullOrEmpty(customerSession))
            {
                return RedirectToAction("customerLogin", "customer");
            }

            int customerId = int.Parse(customerSession);

            var wishlistProducts = _context.tbl_wishlist
                .Include(w => w.Product).AsNoTracking()
                .Where(w => w.customer_id == customerId)
                .Select(w => w.Product)
                .ToList();

            return View(wishlistProducts);
        }
        public IActionResult sellProduct()
        {
            return View();
        }



    }


}
