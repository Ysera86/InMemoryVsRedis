using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Models;
using RedisExchangeAPI.Web.Services;
using StackExchange.Redis;
using System.Diagnostics;

namespace RedisExchangeAPI.Web.Controllers
{
    /// <summary>
    /// C#.Dictionary
    /// </summary>
    public class HashTypeController : Controller
    {
        private readonly ILogger<HashTypeController> _logger;
        private readonly RedisService _redisService;

        private readonly IDatabase db;
        private string hashkey = "dictionarynames";

        public HashTypeController(ILogger<HashTypeController> logger, RedisService redisService)
        {
            _logger = logger;
            _redisService = redisService;

            db = _redisService.GetDb(4);
        }

        public IActionResult Index()
        {
            Dictionary<string,string> list = new Dictionary<string, string>();

            if (db.KeyExists(hashkey))
            {
                //db.HashGet(hashkey, "pen");
                //db.HashExists(hashkey,"pen")

                db.HashGetAll(hashkey).ToList().ForEach(x =>
                {
                    list.Add(x.Name.ToString(), x.Value.ToString()); 
                });

            }

            return View(list);
        }

        [HttpPost]
        public IActionResult Add(string key, string value)
        {
            
            db.KeyExpire(hashkey, DateTime.Now.AddMinutes(5));
            

            db.HashSet(hashkey, key, value);

            return RedirectToAction("Index");
        }




        public IActionResult DeleteItem(string key)
        {
            db.HashDelete(hashkey, key);

            return RedirectToAction("Index");
        }


    }
}