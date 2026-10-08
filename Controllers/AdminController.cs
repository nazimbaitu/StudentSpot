using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
//using Pak.Models;
using StudentSpot.Models;

namespace StudentSpot.Controllers
{
    public class AdminController : Controller
    {
        private myData _context;

        private IWebHostEnvironment _env;


        public AdminController(myData context, IWebHostEnvironment env)
        {
            _env = env;


            _context = context;
        }
        public IActionResult Index()
        {
            string admin_session = HttpContext.Session.GetString("admin_session");

            if (admin_session != null)
            {
                var recentOrders = _context.tbl_baynow
                    .Include(b => b.Cart)
                        .ThenInclude(c => c.Products)
                    .Include(b => b.Cart)
                        .ThenInclude(c => c.customers)
                    .OrderByDescending(b => b.order_i)
                    .Take(4)
                    .ToList();

                // ✅ Calculate Total Sales using quantity * price
                decimal totalSales = 0;

                foreach (var order in recentOrders)
                {
                    if (order.Cart != null && order.Cart.Products != null)
                    {
                        var cart = order.Cart;
                        var product = cart.Products;

                        if (decimal.TryParse(product.product_price, out decimal price))
                        {
                            totalSales += price * cart.product_quantity;
                        }
                    }
                }

                ViewBag.TotalSales = totalSales;

                ViewBag.TotalOrders = _context.tbl_baynow.Count();
                ViewBag.TotalProducts = _context.tbl_product.Count();
                ViewBag.totalCustomer = _context.tbl_customer.Count(); // ✅ Check here

                return View(recentOrders);  // Make sure your view uses ViewBag
            }
            else
            {
                return RedirectToAction("login");
            }
        }

        //=====================================================================================================================
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string adminEmail, string adminPassword)
        {
            var admin = _context.tbl_admin
                .FirstOrDefault(a => a.admin_email == adminEmail);

            if (admin != null && admin.admin_password == adminPassword)
            {
                HttpContext.Session.SetString("admin_session", admin.admin_id.ToString());
                return RedirectToAction("Index"); // Dashboard pe redirect
            }
            else
            {
                ViewBag.Error = "Invalid email or password";
                return View();
            }
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("admin_session");
            return RedirectToAction("Login");
        }
        //=========================================================================================================================================

        [HttpGet]
        public IActionResult Profile()

        {
            var adminId = HttpContext.Session.GetString("admin_session");
            if (string.IsNullOrEmpty(adminId))
            {
                return RedirectToAction("Login", "Admin");
            }

            var row = _context.tbl_admin
                .Where(a => a.admin_id == int.Parse(adminId))
                .ToList();

            return View(row);
        }

        [HttpPost]
        public IActionResult Profile(Admin admin, IFormFile admin_image_file)
        {
           
            //--
            var existingAdmin = _context.tbl_admin.FirstOrDefault(a => a.admin_id == admin.admin_id);
            if (existingAdmin == null)
            {
                return NotFound();
            }

            // Update fields
            existingAdmin.admin_name = admin.admin_name;
            existingAdmin.admin_email = admin.admin_email;
            existingAdmin.admin_password = admin.admin_password;

            // Handle image upload
            if (admin_image_file != null && admin_image_file.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/admin_image");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(admin_image_file.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    admin_image_file.CopyTo(stream);
                }

                existingAdmin.admin_image = uniqueFileName;
            }

            _context.tbl_admin.Update(existingAdmin);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        //====================================================================================================================================
        // ✅ Show all customers
        public IActionResult FetchCustomer()

        {

            var customers = _context.tbl_customer.ToList();
            return View(customers);
        }

        // ✅ Customer Details
        public IActionResult CustomerDetails(int id)
        {
            var customer = _context.tbl_customer.FirstOrDefault(c => c.customer_id == id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // ✅ Update GET
        public IActionResult CustomerUpdate(int id)
        {
            var customer = _context.tbl_customer.FirstOrDefault(c => c.customer_id == id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // ✅ Update POST
        [HttpPost]
        public IActionResult UpdateCustomer(Customer customer, IFormFile? ImageFile)
        {
            if (customer == null)
                return BadRequest();

            var existingCustomer = _context.tbl_customer.FirstOrDefault(c => c.customer_id == customer.customer_id);
            if (existingCustomer == null)
                return NotFound();

            // Update fields
            existingCustomer.customer_name = customer.customer_name;
            existingCustomer.customer_email = customer.customer_email;
            existingCustomer.customer_password = customer.customer_password;
            existingCustomer.customer_phone = customer.customer_phone;
            existingCustomer.customer_adress = customer.customer_adress;
            existingCustomer.customer_gender = customer.customer_gender;
            existingCustomer.customer_country = customer.customer_country;

            // ✅ Image Upload
            if (ImageFile != null && ImageFile.Length > 0)
            {
                string uploadFolder = Path.Combine(_env.WebRootPath, "customer_image");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                string filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }

                // ✅ Purani image delete karna (agar ho to)
                if (!string.IsNullOrEmpty(existingCustomer.customer_image))
                {
                    string oldImagePath = Path.Combine(uploadFolder, existingCustomer.customer_image);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                existingCustomer.customer_image = uniqueFileName;
            }

            _context.tbl_customer.Update(existingCustomer);
            _context.SaveChanges();

            return RedirectToAction("FetchCustomer");
        }

        // ✅ Delete
        public IActionResult CustomerDelete(int id)
        {
            var customer = _context.tbl_customer.FirstOrDefault(c => c.customer_id == id);
            if (customer == null)
            {
                return NotFound();
            }

            // Delete image from folder
            if (!string.IsNullOrEmpty(customer.customer_image))
            {
                string uploadFolder = Path.Combine(_env.WebRootPath, "customer_image");
                string oldImagePath = Path.Combine(uploadFolder, customer.customer_image);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            _context.tbl_customer.Remove(customer);
            _context.SaveChanges();

            return RedirectToAction("FetchCustomer");
        }

        ////===================================================================================================================
        ////====================================================================================
        public IActionResult fetchCategory()
        {
            return View(_context.tbl_category.ToList());
        }

        public IActionResult addCategory()
        {
            return View();
        }

        [HttpPost]
        public IActionResult addCategory(Category cat)
        {
            _context.tbl_category.Add(cat);
            _context.SaveChanges();
            return RedirectToAction("fetchCategory");
        }

        public IActionResult updateCategory(int id)
        {
            var category = _context.tbl_category.Find(id);
            return View(category);
        }

        [HttpPost]
        public IActionResult updateCategory(Category cat)
        {
            _context.tbl_category.Update(cat);
            _context.SaveChanges();
            return RedirectToAction("fetchCategory");
        }
        public IActionResult deletePermissionCategory(int id)
        {
            return View(_context.tbl_category.FirstOrDefault(c => c.category_id == id));
        }

        public IActionResult deleteCategory(int id)
        {
            var category = _context.tbl_category.Find(id);
            _context.tbl_category.Remove(category);
            _context.SaveChanges();
            return RedirectToAction("fetchCategory");
        }
        //=====================================================================================================================
        public IActionResult fetchProduct()
        {
            return View(_context.tbl_product.ToList());
        }

        public IActionResult addProduct()
        {
            List<Category> categories = _context.tbl_category.ToList();
            ViewData["category"] = categories;

            return View();  
        }
        [HttpPost]
        public IActionResult addProduct(Product prod, IFormFile product_image)
        {
            if (product_image != null && product_image.Length > 0)
            {
                string imageName = Path.GetFileName(product_image.FileName);
                string folderName = "products_image";
                string folderPath = Path.Combine(_env.WebRootPath, folderName);

                // Make sure folder exists
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Correct path: wwwroot/pro_image/image.jpg
                string imagePath = Path.Combine(folderPath, imageName);

                // Save image to folder
                using (FileStream fs = new FileStream(imagePath, FileMode.Create))
                {
                    product_image.CopyTo(fs);
                }

                // Save filename (just name, not full path) in DB
                prod.product_image = imageName;
            }

            _context.tbl_product.Add(prod);
            _context.SaveChanges();

            return RedirectToAction("fetchProduct");
        }
        public IActionResult ProductDetails(int id)
        {
            return View(_context.tbl_product.Include(p => p.Category).FirstOrDefault(p => p.product_id == id));

        }
        
        public IActionResult deletePermissionProduct(int id)
        {
            return View(_context.tbl_product.FirstOrDefault(p => p.product_id == id));
        }
        public IActionResult deleteProduct(int id)
        {
            var product = _context.tbl_product.Find(id);
            _context.tbl_product.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("fetchProduct");
        }
        // GET method - for loading form
        [HttpGet]
        public IActionResult updateProduct(int id)
        {

            List<Category> categories = _context.tbl_category.ToList();
            ViewData["category"] = categories;
            
            var product = _context.tbl_product.Find(id);
            ViewBag.selectedcategoryId = product.cat_id;
            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> updateProduct(Product product, IFormFile productImage, string existingImage)
        {
            // Get the existing product from the database
            var existingProduct = _context.tbl_product.FirstOrDefault(p => p.product_id == product.product_id);

            if (existingProduct == null)
            {
                return NotFound();
            }

            // Update fields manually
            existingProduct.product_name = product.product_name;
            existingProduct.product_price = product.product_price;
            existingProduct.product_description = product.product_description;
            existingProduct.cat_id = product.cat_id;

            if (productImage != null && productImage.Length > 0)
            {
                // Generate unique name
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(productImage.FileName);
                string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/products_image", fileName);

                // Save new image
                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await productImage.CopyToAsync(stream);
                }

                // Delete old image if exists
                if (!string.IsNullOrEmpty(existingImage))
                {
                    string oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/products_image", existingImage);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                existingProduct.product_image = fileName;
            }
            else
            {
                // Keep existing image
                existingProduct.product_image = existingImage;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("fetchProduct");
        }
        //===================================================================================================================
            // GET: Admin/fechBlog
            public IActionResult fechBlog()
            {
                var blogs = _context.tbl_bestblog.ToList();
                return View(blogs); // This will send the list of blogs to your view
            }

            // GET: Admin/AddBlog
            public IActionResult AddBlog()
            {
                return View();
            }






        // POST: Admin/AddBlog
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBlog(Bestblog model, IFormFile blog_image)
        {
            if (ModelState.IsValid)
            {
                // Handle image upload if file is provided
                if (blog_image != null && blog_image.Length > 0)
                {
                    // Ensure folder exists
                    string uploadFolder = Path.Combine(_env.WebRootPath, "blog_image");
                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    // Generate unique file name
                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(blog_image.FileName);
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);

                    // Save the file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await blog_image.CopyToAsync(fileStream);
                    }

                    model.blog_image = uniqueFileName;
                }

                // Save to database
                _context.tbl_bestblog.Add(model);
                await _context.SaveChangesAsync();

                ViewBag.SuccessMessage = "Blog added successfully!";
                ModelState.Clear(); // Clear the form
                return View(); // return same view with message
            }

            return View(model); // In case of validation errors
        }








        // GET: Admin/UpdateBlog/{id}
        public IActionResult UpdateBlog(int id)
            {
                var blog = _context.tbl_bestblog.FirstOrDefault(b => b.blog_id == id);
                if (blog == null)
                {
                    return NotFound();
                }
                return View(blog);
            }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBlog(Bestblog model, IFormFile blogImage, string existingImage)
        {
            if (ModelState.IsValid)
            {
                var blogToUpdate = _context.tbl_bestblog.FirstOrDefault(b => b.blog_id == model.blog_id);
                if (blogToUpdate == null)
                {
                    return NotFound();
                }

                blogToUpdate.blog_name = model.blog_name;
                blogToUpdate.blog_description = model.blog_description;

                if (blogImage != null && blogImage.Length > 0)
                {
                    // Save new image
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "blog_image");
                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(blogImage.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await blogImage.CopyToAsync(fileStream);
                    }

                    // Optionally delete old image
                    if (!string.IsNullOrEmpty(existingImage))
                    {
                        string oldImagePath = Path.Combine(uploadsFolder, existingImage);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    blogToUpdate.blog_image = uniqueFileName;
                }
                else
                {
                    // Keep existing image
                    blogToUpdate.blog_image = existingImage;
                }

                _context.Update(blogToUpdate);
                await _context.SaveChangesAsync();

                return RedirectToAction("fechBlog");
            }

            return View(model);
        }

        // GET: Admin/BlogDetails/{id}
        public IActionResult BlogDetails(int id)
            {
                var blog = _context.tbl_bestblog.FirstOrDefault(b => b.blog_id == id);
                if (blog == null)
                {
                    return NotFound();
                }
                return View(blog);
            }

            // GET: Admin/DeleteBlog/{id}
            public IActionResult DeleteBlog(int id)
            {
                var blog = _context.tbl_bestblog.FirstOrDefault(b => b.blog_id == id);
                if (blog == null)
                {
                    return NotFound();
                }

                _context.tbl_bestblog.Remove(blog);
                _context.SaveChanges();

                return RedirectToAction("fechBlog");
            }



        //======================================================================

        public IActionResult Feedback()
        {
            var feedbackList = _context.tbl_feedback.ToList(); // call ToList()
            return View(feedbackList);
        }
        public IActionResult deletefeedback(int id )
        {
            var feedback = _context.tbl_feedback.FirstOrDefault(f => f.id == id);
            if (feedback == null)
            {
                return NotFound();
            }

            _context.tbl_feedback.Remove(feedback);
            _context.SaveChanges();

            return RedirectToAction("Feedback");
        }

        //============================================================================================================

        public IActionResult fetchCart()
        {
            var cartList = _context.tbl_cart.Include(c=>c.Products).Include(c=>c.customers).ToList() ;
            return View(cartList);

        }

        public IActionResult EditCart(int id)
        {
            var cart = _context.tbl_cart.FirstOrDefault(c => c.cart_id == id);
            if (cart == null)
            {
                return NotFound();
            }
            return View(cart);
        }

        [HttpPost]
        public IActionResult EditCart(int cart_id, int status_id)
        {
            var existingCart = _context.tbl_cart.FirstOrDefault(c => c.cart_id == cart_id);
            if (existingCart != null)
            {
                existingCart.status_id = status_id;
                _context.SaveChanges();
                return RedirectToAction("fetchCart");
            }




            return NotFound();
        }
        // Direct GET request se delete karna
        // GET: Delete Cart Item by id
        public IActionResult DeleteCart(int id)





        {
            var cart = _context.tbl_cart.FirstOrDefault(c => c.cart_id == id);
            if (cart == null)
            {
                return NotFound();
            }

            _context.tbl_cart.Remove(cart);
            _context.SaveChanges();

            return RedirectToAction("fetchCart");
        }

        //=========================================================================================

        public IActionResult Dashboard()
        {
            var bayOrders = _context.tbl_baynow
                .Include(b => b.Cart)
                    .ThenInclude(c => c.Products)
                .Include(b => b.Cart)
                    .ThenInclude(c => c.customers)
                .ToList();

            return View(bayOrders);
        }

        public IActionResult FetchBayOrders()
        {
            var bayOrders = _context.tbl_baynow
                .Include(b => b.Cart)
                    .ThenInclude(c => c.Products)
                .Include(b => b.Cart)
                    .ThenInclude(c => c.customers)
                .ToList();

            return View(bayOrders);
        }

        [HttpPost]
        public IActionResult ApproveOrder(int id)
        {
            var order = _context.tbl_baynow.FirstOrDefault(o => o.order_i == id);

            if (order == null)
                return NotFound();

            if (order.Bay_status_id == 0)
            {
                order.Bay_status_id = 1;
                _context.SaveChanges();
            }

            return RedirectToAction("FetchBayOrders");
        }

        public IActionResult OrderDetails(int id)
        {
            var order = _context.tbl_baynow
                .Include(b => b.Cart)
                    .ThenInclude(c => c.Products)
                .Include(b => b.Cart)
                    .ThenInclude(c => c.customers)
                .FirstOrDefault(o => o.order_i == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

    }
}
