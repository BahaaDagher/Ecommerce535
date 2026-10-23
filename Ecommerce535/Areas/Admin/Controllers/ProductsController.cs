using Ecommerce535.Data;
using Ecommerce535.Repositories;
using Ecommerce535.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace Ecommerce535.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        //private readonly ApplicationDbContext _context = new ApplicationDbContext();
        private readonly IRepository<Product> _productReopository; //= new Repository<Product>();
        private readonly IRepository<Category> _categoryReopository;// = new Repository<Category>();
        private readonly IRepository<Brand> _BrandReopository;// = new Repository<Brand>();
        private readonly IProductSubImageRepository _ProductSubImageReopository;// = new ProductSubImageRepository();
        private readonly IProductColorRepository _ProductColorReopository;// = new ProductColorRepository();

        public ProductsController(IRepository<Product> productReopository, IRepository<Category> categoryReopository, IRepository<Brand> brandReopository, IProductSubImageRepository productSubImageReopository, IProductColorRepository productColorReopository)
        {
            _productReopository = productReopository;
            _categoryReopository = categoryReopository;
            _BrandReopository = brandReopository;
            _ProductSubImageReopository = productSubImageReopository;
            _ProductColorReopository = productColorReopository;
        }

        public async Task<IActionResult> Index(FilterProductVM filter)
        {
            //var products = _context.Products.AsQueryable();
            //products = products.Include(p => p.Category).Include(p => p.Brand);
            var products = await _productReopository.GetAllAsync(includes: [p => p.Category , p => p.Brand]);
            // filter 
            if (filter.ProductName != null)
            {
                products = products.Where(p => p.Name.Contains(filter.ProductName));
            }
            if (filter.MinPrice > 0)
            {
                products = products.Where(p => p.Price >= filter.MinPrice);
            }
            if (filter.MaxPrice > 0)
            {
                products = products.Where(p => p.Price <= filter.MaxPrice);
            }
            if (filter.CategoryId > 0)
            {
                products = products.Where(p => p.CategoryId == filter.CategoryId);
            }
            if (filter.BrandId > 0)
            {
                products = products.Where(p => p.BrandId == filter.BrandId);
            }
            if (filter.IsLowQuanity)
            {
                //products = products.Where(p => p.Quantity < 40);
                products = products.OrderBy(p => p.Quantity);
            }
            //ViewBag.Categories = _context.Categories;
            ViewBag.Categories = await _categoryReopository.GetAllAsync();
            //ViewBag.Brands = _context.Brands;
            ViewBag.Brands = await _BrandReopository.GetAllAsync();
            ViewBag.TotalPages = (int)Math.Ceiling(products.Count() / 8.0);

            // page = 1 
            products = products.Skip((filter.page - 1) * 8).Take(8);
            return View(products.AsEnumerable());
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            //var categories = _context.Categories.ToList();
            var categories = await _categoryReopository.GetAllAsync();
            //var brands = _context.Brands.ToList();
            var brands = await _BrandReopository.GetAllAsync();
            return View(new ProductVM()
            {
                Categories = categories.ToList(),
                Brands = brands.ToList()
            });
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFile ImageFile, List<IFormFile> SubImages, List<string> Colors)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                //a3f1c2d4-7b8e-4a9f-9c2d-1e5f6a7b8c9d.jpg
                //var fileName = Guid.NewGuid().ToString() +Path.GetExtension(ImageFile.FileName);
                //a3f1c2d4-7b8e-4a9f-9c2d-1e5f6a7b8c9d-1.jpg
                var fileName = Guid.NewGuid().ToString() + "-" + ImageFile.FileName;
                //var filePath = "D:\\EraaSoft\\535\\Project\\Ecommerce535\\Ecommerce535\\wwwroot\\images\\";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\", fileName);


                using (var stream = System.IO.File.Create(filePath))
                {
                    ImageFile.CopyTo(stream);
                }
                product.MainImg = fileName;
            }
            //var SavedProduct = _context.Products.Add(product);
            var SavedProduct = await _productReopository.AddAsync(product);
            //_context.SaveChanges();
            await _productReopository.CommitAsync();

            if (SubImages != null && SubImages.Count > 0)
            {
                foreach (var image in SubImages)
                {
                    if (image != null && image.Length > 0)
                    {
                        // Save in wwwroot
                        var fileName = Guid.NewGuid().ToString() + "-" + image.FileName;
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\", fileName);

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            image.CopyTo(stream);
                        }

                        // save in DB 
                        //_context.ProductSubImages.Add(new ProductSubImage()
                        await _ProductSubImageReopository.AddAsync(new ProductSubImage()
                        {
                            Img = fileName,
                            ProductId = SavedProduct.Entity.Id
                        });
                    }
                }
                await _ProductSubImageReopository.CommitAsync();
            }
            if (Colors != null && Colors.Count > 0)
            {
                foreach (var color in Colors)
                {
                    // save in DB 
                    //_context.ProductColors.Add(new ProductColor()
                    await _ProductColorReopository.AddAsync(new ProductColor()
                    {
                        Color = color,
                        ProductId = SavedProduct.Entity.Id
                    });
                }
                //_context.SaveChanges();
                await _ProductColorReopository.CommitAsync(); 
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            //var product = _context.Products.FirstOrDefault(c => c.Id == id);
            var product = await _productReopository.GetOneAsync(c => c.Id == id);
            if (product == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }

            return View(new ProductVM()
            {
                Product = product,
                Categories = (await _categoryReopository.GetAllAsync()).ToList(),
                Brands = (await _BrandReopository.GetAllAsync()).ToList(),
                //ProductSubImages = _context.ProductSubImages.Where(p => p.ProductId == id).ToList(),
                ProductSubImages = (await _ProductSubImageReopository.GetAllAsync(p => p.ProductId == id)).ToList(),
                //ProductColors = _context.ProductColors.Where(p => p.ProductId == id).ToList()
                ProductColors = (await _ProductColorReopository.GetAllAsync(p => p.ProductId == id)).ToList()
            });   
        }
        [HttpPost]
        public async Task<IActionResult> Update(Product product, IFormFile ImageFile , List<IFormFile> SubImages, List<string> Colors)
        {
            //var productInDb = _context.Products.AsNoTracking().FirstOrDefault(b => b.Id == product.Id);
            var productInDb = await _productReopository.GetOneAsync(b => b.Id == product.Id , isTracked: false);

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + "-" + ImageFile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    ImageFile.CopyTo(stream);
                }
                product.MainImg = fileName;


                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\", productInDb.MainImg);

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }
            else
            {
                product.MainImg = productInDb.MainImg;
            }
            //_context.Products.Update(product);
            //_context.SaveChanges();
            _productReopository.Update(product);
            await _productReopository.CommitAsync();

            if (SubImages != null && SubImages.Count > 0)
            {
                foreach (var image in SubImages)
                {
                    //var oldSubImages = _context.ProductSubImages.Where(p=>p.ProductId == product.Id);
                    var oldSubImages = await _ProductSubImageReopository.GetAllAsync(p => p.ProductId == product.Id);
                    // remove from wwwroot
                    foreach(var item in oldSubImages)
                    {
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\", item.Img);

                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    // remove from DB 
                    // ****/
                    //_context.ProductSubImages.RemoveRange(oldSubImages);
                    _ProductSubImageReopository.RemoveRange(oldSubImages);

                    if (image != null && image.Length > 0)
                    {
                        // Save in wwwroot
                        var fileName = Guid.NewGuid().ToString() + "-" + image.FileName;
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\", fileName);

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            image.CopyTo(stream);
                        }

                        // save in DB 
                        //_context.ProductSubImages.Add(new ProductSubImage()
                        await _ProductSubImageReopository.AddAsync(new ProductSubImage()
                        {
                            Img = fileName,
                            ProductId = product.Id
                        });
                    }
                }
                //_context.SaveChanges();
                await _ProductSubImageReopository.CommitAsync();
            }
            if (Colors != null && Colors.Count > 0)
            {
                //var oldColors = _context.ProductColors.Where(p => p.ProductId == product.Id);
                var oldColors = await _ProductColorReopository.GetAllAsync(p => p.ProductId == product.Id);

                // remove from DB 
                //_context.ProductColors.RemoveRange(oldColors);
                _ProductColorReopository.RemoveRange(oldColors); 
                // ****/
                foreach (var color in Colors)
                {
                    // save in DB 
                    //_context.ProductColors.Add(new ProductColor()
                    await _ProductColorReopository.AddAsync(new ProductColor()
                    {
                        Color = color,
                        ProductId = product.Id
                    });
                }
                //_context.SaveChanges();
                await _ProductColorReopository.CommitAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            //var product = _context.Products.FirstOrDefault(c => c.Id == id);
            var product = await _productReopository.GetOneAsync(c => c.Id == id);
            if (product == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\", product.MainImg);

            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }

            // remove subimages from wwwroot
            //var oldSubImages = _context.ProductSubImages.Where(p => p.ProductId == product.Id);
            var oldSubImages = await _ProductSubImageReopository.GetAllAsync(p => p.ProductId == product.Id); 
            foreach (var item in oldSubImages)
            {
                var old = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\", item.Img);

                if (System.IO.File.Exists(old))
                {
                    System.IO.File.Delete(old);
                }
            }

            //_context.Products.Remove(product);
            //_context.SaveChanges();
            _productReopository.Remove(product);
            await _productReopository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
