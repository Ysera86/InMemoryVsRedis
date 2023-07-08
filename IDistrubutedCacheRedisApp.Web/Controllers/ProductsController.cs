using IDistrubutedCacheRedisApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

namespace IDistrubutedCacheRedisApp.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IDistributedCache _distributedCache;

        public ProductsController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        //public IActionResult Index()
        //{
        //    /*
        //      127.0.0.1:6379> KEYS * > 
        //     */
        //    DistributedCacheEntryOptions cacheOptions = new DistributedCacheEntryOptions();
        //    cacheOptions.AbsoluteExpiration = DateTime.Now.AddMinutes(1);

        //    _distributedCache.SetString("name", "Merve", cacheOptions);

        //    /*
        //     127.0.0.1:6379> KEYS *
        //    name        

        //     type name > hash

        //    hget name data > Merve

        //     */

        //    return View();
        //}

        public async Task< IActionResult > Index()
        {
            /*
              127.0.0.1:6379> KEYS * > 
             */
            DistributedCacheEntryOptions cacheOptions = new DistributedCacheEntryOptions();
            cacheOptions.AbsoluteExpiration = DateTime.Now.AddMinutes(1);

            await _distributedCache.SetStringAsync("surname", "Ugursac", cacheOptions);

            /*
            127.0.0.1:6379> KEYS *
            surname
            127.0.0.1:6379> hget surname data
            Ugursac
            127.0.0.1:6379>

             */

            #region 1. yol json str

            Product product = new Product { Id = 1, Name = "Kalem", Price = 100 };
            var jsonProduct = JsonSerializer.Serialize(product);

            await _distributedCache.SetStringAsync("product:1", jsonProduct, cacheOptions);

            #endregion

            #region 2. yol binary str

            Product product2 = new Product { Id = 2, Name = "Kitap", Price = 200 };
            var jsonProduct2 = JsonSerializer.Serialize(product2);

            Byte[] byteProduct = Encoding.UTF8.GetBytes(jsonProduct2);

            await _distributedCache.SetAsync("product:2", byteProduct, cacheOptions);

            #endregion

            /*
             KEYS *
            product:2
            product:1
            surname

            127.0.0.1:6379> hget product:1 data
            {"Id":1,"Name":"Kalem","Price":100}
            127.0.0.1:6379> hget product:2 data
            {"Id":2,"Name":"Kitap","Price":200}
             
             */


            return View();
        }

        public IActionResult Show()
        {
            ViewBag.name = _distributedCache.GetString("name");

            //ViewBag.product1 = _distributedCache.GetString("product:1");

            var pjson = _distributedCache.GetString("product:1");
            Product product = JsonSerializer.Deserialize<Product>(pjson);

            Byte[] bytePjson = _distributedCache.Get("product:2");
            var pjson2 = Encoding.UTF8.GetString(bytePjson);
            Product product2 = JsonSerializer.Deserialize<Product>(pjson2);

            ViewBag.product = product;
            ViewBag.product2 = product2;
            /*
             KEYS *
            product:1
            surname

            hget product:1 data
            {"Id":1,"Name":"Kalem","Price":100}  
            hget surname data
            Ugursac
            */

            return View();
        }

        public IActionResult Remove()
        {
            _distributedCache.Remove("name");
            /*
              hget name data > 
              KEYS * > 
             */

            _distributedCache.Remove("product:1");
            /*
              KEYS *
                surname
             */

            ViewBag.name = _distributedCache.GetString("name");
            ViewBag.product1 = _distributedCache.GetString("product:1");



            return View();
        }
  
        public IActionResult ImageCache()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Impala.png");
            byte[] imgByte = System.IO.File.ReadAllBytes(path);

            _distributedCache.Set("image", imgByte);



            return View();
        }

        public IActionResult ImageCacheShow()
        {
            byte[] imgByte = _distributedCache.Get("image");

            return File(imgByte, "image/png");
         
       
        }


    }
}
