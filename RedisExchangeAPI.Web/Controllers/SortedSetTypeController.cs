using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Models;
using RedisExchangeAPI.Web.Services;
using StackExchange.Redis;
using System.Diagnostics;

namespace RedisExchangeAPI.Web.Controllers
{
    /// <summary>
    /// Score değerine göre 
    /// </summary>
    public class SortedSetTypeController : Controller
    {
        private readonly ILogger<SetTypeController> _logger;
        private readonly RedisService _redisService;

        private readonly IDatabase db;
        private string listKey = "sortedsetnames";

        public SortedSetTypeController(ILogger<SetTypeController> logger, RedisService redisService)
        {
            _logger = logger;
            _redisService = redisService;

            db = _redisService.GetDb(3);
        }

        public IActionResult Index()
        {
            HashSet<string> list = new HashSet<string>();

            if (db.KeyExists(listKey))
            {
                //// SortedSetScanredis sırasıyla getirir dataları
                db.SortedSetScan(listKey).ToList().ForEach(x =>
                {
                    list.Add(x.ToString()); // x.Element.ToString() // bu şekilde direk x verince name:score şeklinde geliyor name alanı silmede hata alınıyor, bu nedenle de x.Element verdiğimizde silme çalışyor, x verdiğimizde bu listelemede name:score şeklinde görüntüleyebiliyoruz.
                });

                //db.SortedSetRangeByRank(listKey, order: Order.Ascending).ToList().ForEach(x =>
                //{
                //    list.Add(x.ToString());
                //});

                //db.SortedSetRangeByRank(listKey,0,5 ,order: Order.Ascending).ToList().ForEach(x =>
                //{
                //    list.Add(x.ToString());
                //}); 
            }

            return View(list);
        }

        [HttpPost]
        public IActionResult Add(string name, int score)
        {
            
            db.KeyExpire(listKey, DateTime.Now.AddMinutes(5));

       
            //if (!db.KeyExists(listKey))
            //{
            //    db.KeyExpire(listKey, DateTime.Now.AddMinutes(5)); 
            //}

            db.SortedSetAdd(listKey, name, score);

            return RedirectToAction("Index");
        }




        public IActionResult DeleteItem(string name)
        {
            db.SortedSetRemove(listKey, name);

            return RedirectToAction("Index");
        }


    }
}