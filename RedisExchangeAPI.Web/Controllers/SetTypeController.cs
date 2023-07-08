using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Models;
using RedisExchangeAPI.Web.Services;
using StackExchange.Redis;
using System.Diagnostics;

namespace RedisExchangeAPI.Web.Controllers
{
    /// <summary>
    /// C#.HashSet
    /// </summary>
    public class SetTypeController : Controller
    {
        private readonly ILogger<SetTypeController> _logger;
        private readonly RedisService _redisService;

        private readonly IDatabase db;
        private string listKey = "hashnames";

        public SetTypeController(ILogger<SetTypeController> logger, RedisService redisService)
        {
            _logger = logger;
            _redisService = redisService;

            db = _redisService.GetDb(2);
        }

        public IActionResult Index()
        {
            HashSet<string> list = new HashSet<string>();

            if (db.KeyExists(listKey))
            {
                db.SetMembers(listKey).ToList().ForEach(x =>
                {
                    list.Add(x.ToString());
                });
            }

            return View(list);
        }

        [HttpPost]
        public IActionResult Add(string name)
        {
            // -> sliding özelliği ilgili data her get edildiğinde bu şekilde yeniden set edilerek kazandırılabilir
            db.KeyExpire(listKey, DateTime.Now.AddMinutes(5));

            //// -> bu absolute.
            //if (!db.KeyExists(listKey))
            //{
            //    db.KeyExpire(listKey, DateTime.Now.AddMinutes(5)); 
            //}

            db.SetAdd(listKey, name); // aynı name eklenmicek: unique

            //db.SetRandomMembers(listKey, 2);
            //db.SetPop(listKey);
            //db.SetLength(listKey);
            //....

            return RedirectToAction("Index");
        }




        //public IActionResult DeleteItem(string name)
        //{
        //    db.SetRemove(listKey, name);

        //    return RedirectToAction("Index");
        //}

        public async Task<IActionResult> DeleteItem(string name)
        {
            await db.SetRemoveAsync(listKey, name);

            return RedirectToAction("Index");
        }
    }
}