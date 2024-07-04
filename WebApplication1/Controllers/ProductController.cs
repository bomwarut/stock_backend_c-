using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : Controller
    {
        DatabaseContext db;

        [HttpGet(Name = "/")]
        public Object Index()
        {
            db= new DatabaseContext();
            /*ADD*/
            /*      product newProduct = new product();
                      newProduct.product_price = 990;
                      newProduct.product_name = "coke";
                      newProduct.product_code = "91223";
                      db.Add(newProduct);
                      db.SaveChanges();*/

            /*      product targetWithSelect = db.products.FirstOrDefault((e) => e.product_id == 1 || e.product_code.Equals("999111"));*/
    /*        product target = db.products.Find(2);*/

            /*UPDATE
  /*          target.product_name = target.product_name + " | " + new Random().NextDouble() * 5.0f;
            db.Update(target);
            db.SaveChanges()*/;

            // List<product>
            List<product> products = db.products.ToList();
            return new { 
                        showdata = products
            };
        }
    }
}
