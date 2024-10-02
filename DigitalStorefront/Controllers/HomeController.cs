using DigitalStorefront.Data;
using DigitalStorefront.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DigitalStorefront.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DSDbContext _context;

        public HomeController(ILogger<HomeController> logger, DSDbContext pMSDbContext)
        {
            _logger = logger;
            _context = pMSDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.ProductDetails)
                .ToListAsync();
            return View(orders);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public async Task<IActionResult> Create()
        {
           
            string lastOrderCode = await _context.Orders
                .OrderByDescending(o => o.OrderCode)
                .Select(o => o.OrderCode)
                .FirstOrDefaultAsync();

            // Default order code if none exists
            string newOrderCode;
            if (string.IsNullOrWhiteSpace(lastOrderCode)) {
                newOrderCode = "OrderNO:0001";
            }
            else {
                
                string numericPart = lastOrderCode.Substring("OrderNO:".Length);
                if (int.TryParse(numericPart, out int lastNumber)) {
                    int newNumber = lastNumber + 1;
                    newOrderCode = $"OrderNO:{newNumber:D4}"; 
                }
                else {
                    
                    newOrderCode = "OrderNO:0001";
                }
            }

            
            var order = new Order {
                OrderCode = newOrderCode,
                OrderDate = DateTime.Now,
                ProductDetails = new List<ProductDetail>
                {
            new ProductDetail
            {
                ProductName = "Sample Product",
                Quantity = 1,
                UnitPrice = 0.00m,
                TotalPrice = 0.00m
            }
        }
            };

            return View(order);
        }



        [HttpPost]
        public async Task<IActionResult> Create(Order order)
        
        {          
            
                
                order.TotalAmount = 0;

                
                foreach (var productDetail in order.ProductDetails)
                {
                    productDetail.Order = order; 
                    productDetail.TotalPrice = productDetail.Quantity * productDetail.UnitPrice;
                    order.TotalAmount += productDetail.TotalPrice;
                }

                
                _context.Orders.Add(order);

                
                await _context.SaveChangesAsync();

                
                return RedirectToAction("Home","Index");
            

            
        }

       
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _context.Orders
                .Include(o => o.ProductDetails)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) {
                return NotFound();
            }

            var viewModel = new OrderEditViewModel {
                Order = order,
                ProductDetails = order.ProductDetails
            };

            return View(viewModel);
        }

        
        [HttpPost]
        public async Task<IActionResult> Edit(OrderEditViewModel viewModel)
        {
            

      
            var order = await _context.Orders
                .Include(o => o.ProductDetails)
                .FirstOrDefaultAsync(o => o.OrderId == viewModel.Order.OrderId);

            if (order == null) {
                return NotFound();
            }

         
            order.OrderCode = viewModel.Order.OrderCode;
            order.CustomerName = viewModel.Order.CustomerName;
            order.Phone = viewModel.Order.Phone;
            order.Address = viewModel.Order.Address;
            order.OrderDate = viewModel.Order.OrderDate;

    
            _context.Products.RemoveRange(order.ProductDetails);

          
            decimal totalAmount = 0;
            foreach (var detail in viewModel.ProductDetails) {
                var productDetail = new ProductDetail {
                    ProductDetailId = detail.ProductDetailId,
                    ProductName = detail.ProductName,
                    Quantity = detail.Quantity,
                    UnitPrice = detail.UnitPrice,
                    TotalPrice = detail.Quantity * detail.UnitPrice
                };
                totalAmount += productDetail.TotalPrice;
                order.ProductDetails.Add(productDetail);
            }


            order.TotalAmount = totalAmount;

            _context.Update(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index","Home");
        }


        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null) {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index","Home");
        }
    }
}
