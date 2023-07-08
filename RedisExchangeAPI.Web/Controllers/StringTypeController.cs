using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Models;
using RedisExchangeAPI.Web.Services;
using StackExchange.Redis;
using System.Diagnostics;

namespace RedisExchangeAPI.Web.Controllers
{
    /// <summary>
    /// db.StringAppend ile tüm methodlara erişebiliyoruz
    /// </summary>
    public class StringTypeController : Controller
    {
        private readonly ILogger<StringTypeController> _logger;
        private readonly RedisService _redisService;

        private readonly IDatabase db;

        public StringTypeController(ILogger<StringTypeController> logger, RedisService redisService)
        {
            _logger = logger;
            _redisService = redisService;

            db = _redisService.GetDb(0);
        }

        public IActionResult Index()
        {
            db.StringSet("name", "Merve Uğursaç");
            db.StringSet("visitor", 100);

            //byte[] image = default(byte[]);
            //db.StringSet("image", image);

            //Product product2 = new Product { Id = 2, Name = "Kitap", Price = 200 };
            //var jsonProduct2 = JsonSerializer.Serialize(product2);
            //db.StringSet("product2", jsonProduct2);

           //. . . 


            //byte[] image = default(byte[]);
            //db.StringSet("image", image);
            /*
             
            127.0.0.1:6379> FLUSHALL
            OK
            127.0.0.1:6379> KEYS *
            (empty array)
            127.0.0.1:6379> KEYS *
            1) "visitor"
            2) "name"
            127.0.0.1:6379> hget name data
            (error) WRONGTYPE Operation against a key holding the wrong kind of value
            127.0.0.1:6379> type name
            string
            127.0.0.1:6379> type visitor
            string
            127.0.0.1:6379> get name
            "Merve U\xc4\x9fursa\xc3\xa7;"
            127.0.0.1:6379> get visitor
            "100"
             
             */


            return View();
        }


        public  IActionResult Show()
        {
            var name = db.StringGet("name");
            if (name.HasValue)
            {
                ViewBag.name = name.ToString();
            }

            //db.StringIncrement("visitor", 10);

            //var visitor = db.StringGet("visitor");
            //if (visitor.HasValue)
            //{
            //    ViewBag.visitor = db.StringIncrement("visitor", 1);
            //}


            // işlemi yap sonucu ver
            var visitorCount = db.StringDecrementAsync("visitor", 10).Result;
            ViewBag.visitor = visitorCount;


            // işlemi yap sonucu bana vermene gerek yok
            db.StringDecrementAsync("visitor", 10).Wait();



            var nameRange = db.StringGetRange("name", 0, 3);
            ViewBag.nameRange = nameRange;

            var nameLength = db.StringLength("name");
            ViewBag.nameLength = nameLength;



            return View();
        }

    }
}