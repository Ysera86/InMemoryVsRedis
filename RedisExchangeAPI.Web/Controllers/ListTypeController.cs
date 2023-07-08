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
    public class ListTypeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RedisService _redisService;

        private readonly IDatabase db;
        private string listKey = "names";

        public ListTypeController(ILogger<HomeController> logger, RedisService redisService)
        {
            _logger = logger;
            _redisService = redisService;

            db = _redisService.GetDb(1);
        }

        public IActionResult Index()
        {
            List<string> list = new List<string>();

            if (db.KeyExists(listKey))
            {
                db.ListRange(listKey).ToList().ForEach(x=> 
                {
                    list.Add(x.ToString());
                }); // 0 -1
            }

            return View(list);
        }

        [HttpPost]
        public  IActionResult Add(string name)
        {
           db.ListRightPush(listKey,name);
            // db.ListLeftPush(listKey,name);

            return RedirectToAction("Index");
        }
        /*
          LRANGE names 0 -1
            xxxxx
            yyyy
         */


       
        public  async Task<IActionResult> DeleteItem(string name) // tag helper ile index.cshtml de asp-route-name ile argument adını verdik
        {
           db.ListRemoveAsync(listKey,name).Wait();


            return RedirectToAction("Index");
        }
        public  IActionResult DeleteFirstItem()
        {
            db.ListLeftPop(listKey);
            //db.ListRightPop(listKey);


            return RedirectToAction("Index");
        }
    }
}