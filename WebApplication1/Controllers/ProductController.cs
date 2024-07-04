using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Text.Json;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : Controller
    {
        private DatabaseContext db;

        public ProductController()
        {
            db = new DatabaseContext();
        }

        [HttpGet(Name = "/product")]
        public Object Index()
        {
            /*ADD*/
            /*product newProduct = new product();
            newProduct.product_price = 990;
            newProduct.product_name = "coke";
            newProduct.product_code = "91223";
            db.Add(newProduct);
            db.SaveChanges();*/

            /*product targetWithSelect = db.products.FirstOrDefault(e => e.product_id == 1 || e.product_code.Equals("999111"));*/
            /*product target = db.products.Find(2);*/

            /*UPDATE*/
            /*target.product_name = target.product_name + " | " + new Random().NextDouble() * 5.0f;
            db.Update(target);
            db.SaveChanges();*/

            // List<product>
            List<product> products = db.products.ToList();
            return new
            {
                showdata = products
            };
        }

        [HttpGet("stocks")]
        public Object GetStocks()
        {
            var stocksWithProductNames = from stock in db.stocks
                                         join product in db.products on stock.product_id equals product.product_id
                                         select new
                                         {
                                             stock.stock_id,
                                             product.product_id,
                                             stock.product_quantity,
                                             stock.stock_update_at,
                                             product.product_name
                                         };
            return new
            {
                showdata = stocksWithProductNames.ToList()
            };
        }

        [HttpGet("history")]
        public Object GetHistory()
        {
            return new
            {
                showdata = db.stock_histories.ToList()
        };
        }

        [HttpPost("addStock")]
        public IActionResult AddStock([FromBody] AddStockRequest request)
        {
            var product = db.products.Find(request.ProductId);
            if (product == null)
            {
                return NotFound(new { success = false, message = "Product not found" });
            }

            var existingStock = db.stocks.FirstOrDefault(s => s.product_id == request.ProductId);
            if (existingStock != null)
            {
                existingStock.product_quantity += request.Quantity;
                existingStock.stock_update_at = DateTime.Now;
                db.stocks.Update(existingStock);
                db.SaveChanges();
                return JsonWithReferenceHandler(new { success = true, updatedStock = existingStock });
            }
            else
            {
                var newStock = new stock
                {
                    product_id = request.ProductId,
                    product_quantity = request.Quantity,
                    stock_update_at = DateTime.Now
                };
                db.stocks.Add(newStock);
                db.SaveChanges();
                return JsonWithReferenceHandler(new { success = true, newStock });
            }
        }

        [HttpPost("withdrawstock")]
        public IActionResult Withdrawstock([FromBody] AddStockRequest request)
        {
            var product = db.products.Find(request.ProductId);
            if (product == null)
            {
                return NotFound(new { success = false, message = "Product not found" });
            }

            var existingStock = db.stocks.FirstOrDefault(s => s.product_id == request.ProductId);
                existingStock.product_quantity -= request.Quantity;
                existingStock.stock_update_at = DateTime.Now;
                db.stocks.Update(existingStock);
                db.SaveChanges();
                return JsonWithReferenceHandler(new { success = true, updatedStock = existingStock });
        }

        private IActionResult JsonWithReferenceHandler(object data)
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };
            var json = JsonSerializer.Serialize(data, options);
            return new ContentResult
            {
                Content = json,
                ContentType = "application/json",
                StatusCode = 200
            };
        }

        public class AddStockRequest
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }

    }
}
