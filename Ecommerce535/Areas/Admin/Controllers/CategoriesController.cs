using Ecommerce535.Data;
using Ecommerce535.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce535.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        //private readonly  ApplicationDbContext _context = new ApplicationDbContext();
        private readonly IRepository<Category> _categoryRepository;//= new Repository<Category>();

        public CategoriesController(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            //var categories = _context.Categories.AsQueryable();
            var categories = await _categoryRepository.GetAllAsync();
            // filter 
            return View(categories.AsEnumerable());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Category());
        }
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error_Notification"] = "invalid data"; 
                return View(category); 
            }
            //_context.Categories.Add(category);
            //_context.SaveChanges();
            await _categoryRepository.AddAsync(category);
            await _categoryRepository.CommitAsync();
            //Response.Cookies.Append("Successful_Notification" , "category created successfully" ); 
            TempData["Successful_Notification"] = "category created successfully"; 
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            //var category = _context.Categories.FirstOrDefault(c=> c.Id == id);
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id);

            if (category == null)
            {
                return RedirectToAction("NotFoundPage" , "Home");
            }
            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Category category)
        {
            //_context.Categories.Update(category);
            //_context.SaveChanges();
            _categoryRepository.Update(category);
            await _categoryRepository.CommitAsync(); 
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            //var category = _context.Categories.FirstOrDefault(c=>c.Id == id);
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id); 
            if (category == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }
            //_context.Categories.Remove(category); 
            //_context.SaveChanges();
            _categoryRepository.Remove(category); 
            await _categoryRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
