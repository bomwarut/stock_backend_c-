﻿using Microsoft.AspNetCore.Mvc;
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

                var historyEntry = new stock_history
                {
                    stock_id = existingStock.stock_id,
                    stock_type = 1,
                    quantity = existingStock.product_quantity,
                    stock_history_update_at = DateTime.Now
                };
                db.stock_histories.Add(historyEntry);
                db.SaveChanges();

                return JsonWithReferenceHandler(new { success = true, updatedStock = existingStock });
            }
            else
            {
                var newStock = new stock
                {
                    product_id = request.ProductId,
                    product_quantity = existingStock.product_quantity,
                    stock_update_at = DateTime.Now
                };
                db.stocks.Add(newStock);
                db.SaveChanges();

                var historyEntry = new stock_history
                {
                    stock_id = newStock.stock_id,
                    stock_type = 1,
                    quantity = existingStock.product_quantity,
                    stock_history_update_at = DateTime.Now
                };
                db.stock_histories.Add(historyEntry);
                db.SaveChanges();

                return JsonWithReferenceHandler(new { success = true, newStock });
            }
        }

        [HttpPost("withdrawStock")]
        public IActionResult WithdrawStock([FromBody] WithdrawStockRequest request)
        {
            var product = db.products.Find(request.ProductId);
            if (product == null)
            {
                return NotFound(new { success = false, message = "Product not found" });
            }
            var existingStock = db.stocks.FirstOrDefault(s => s.product_id == request.ProductId);
            if (existingStock == null || existingStock.product_quantity < request.Quantity)
            {
                return BadRequest(new { success = false, message = "Insufficient stock quantity" });
            }
            existingStock.product_quantity -= request.Quantity;
            existingStock.stock_update_at = DateTime.Now;
            db.stocks.Update(existingStock);
            db.SaveChanges();

            var historyEntry = new stock_history
            {
                stock_id = existingStock.stock_id,
                stock_type = 2,
                quantity = existingStock.product_quantity,
                stock_history_update_at = DateTime.Now
            };
            db.stock_histories.Add(historyEntry);
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

        public class WithdrawStockRequest
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }

    }
}
