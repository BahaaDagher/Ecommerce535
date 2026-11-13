using Ecommerce535.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace Ecommerce535.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Models.Product> _productRepository;

        public CartController(UserManager<ApplicationUser> userManager, IRepository<Cart> cartRepository, IRepository<Models.Product> productRepository)
        {
            _userManager = userManager;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();
            var carts = await _cartRepository.GetAllAsync(c=>c.ApplicationUserId == user.Id , includes: [c=> c.Product]); 
            return View(carts);
        }
        public async Task<IActionResult> AddToCart(int productId  , int count )
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var excitCart = await _cartRepository.GetOneAsync(c=>
            c.ProductId == productId && 
            c.ApplicationUserId == user.Id); 

            if(excitCart != null)
            {
                excitCart.Count += count;
                await _cartRepository.CommitAsync();
                return RedirectToAction(nameof(Index));
            }

            var cart = new Cart()
            {
                ApplicationUserId = user.Id  ,
                ProductId = productId  , 
                Count = count
            };
            await _cartRepository.AddAsync(cart); 
            await _cartRepository.CommitAsync(); 
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> IncrementProduct(int productId )
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var cart = await _cartRepository.GetOneAsync(c =>
            c.ProductId == productId &&
            c.ApplicationUserId == user.Id);

            if (cart == null) return NotFound();

            var product = await _productRepository.GetOneAsync(p=>p.Id == productId );
            if (product is null) return NotFound(); 
            if (cart.Count == product.Quantity)
            {
                // notification can't add more than the Quantity of the Product 
                return RedirectToAction(nameof(Index));
            }
            cart.Count++;
            await _cartRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> DecrementProduct(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var cart = await _cartRepository.GetOneAsync(c =>
            c.ProductId == productId &&
            c.ApplicationUserId == user.Id);

            if (cart == null) return NotFound();

            var product = await _productRepository.GetOneAsync(p => p.Id == productId);
            if (product is null) return NotFound();
            if (cart.Count == 1)
            {
                // notification can't Decrement Less than one 
                return RedirectToAction(nameof(Index));
            }
            cart.Count--;
            await _cartRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var cart = await _cartRepository.GetOneAsync(c =>
            c.ProductId == productId &&
            c.ApplicationUserId == user.Id);

            if (cart == null) return NotFound();

            _cartRepository.Remove(cart); 
            await _cartRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Pay()
        {
            var options = new SessionCreateOptions
            {
                AllowedPaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Checkout/Success",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/Checkout/Cancel",
            };
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound(); 
            var carts = await _cartRepository.GetAllAsync(c=>c.ApplicationUserId == user.Id , includes: [c=>c.Product]); 
            foreach(var item in carts)
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name, 
                            Description = item.Product.Description
                        },
                        UnitAmount = (long)(item.Product.Price * 100), 
                    },
                    Quantity = item.Count ,
                }); 
            }
            var service = new SessionService();
            var session = service.Create(options);

            return Redirect(session.Url);
        }
    }
}
