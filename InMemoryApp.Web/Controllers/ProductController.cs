using InMemoryApp.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Caching.Memory;

namespace InMemoryApp.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IMemoryCache _memoryCache;
        public ProductController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            // cache te her saman key-value

            #region yollar

            //// 1.yol
            //if (string.IsNullOrEmpty(_memoryCache.Get<string>("time")))
            //{
            //    _memoryCache.Set<string>("time", DateTime.Now.ToString());
            //}

            //// 2.yol
            //if (!_memoryCache.TryGetValue("time", out string timeCache))
            //{
            //    _memoryCache.Set<string>("time", DateTime.Now.ToString());
            //}

            //// 3. yol
            //_memoryCache.GetOrCreate<string>("time", x =>
            //{
            //    //x.SlidingExpiration = TimeSpan.FromDays(1);
            //    return DateTime.Now.ToString();
            //});

            ////timeCache 
            ///

            //_memoryCache.Remove("time");
            #endregion

            //IMemoryCache verilen objeleri kendisi seralize ediyor, Redis'te biz yapmalıyız.

            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
            options.AbsoluteExpiration = DateTime.Now.AddSeconds(10);

            //// 10 snyede 1 erişilirse kalır, erişilmezse silinir ama AbsoluteExpiration verdiğimiz için, erişilse de erşilmese de 1 dk snr silinir.
            //// bu nedenle SlidingExpiration verilirken AbsoluteExpiration da verilmeli, yoksa bi süre snr bayat dataya erişilir.
            //options.SlidingExpiration = TimeSpan.FromSeconds(10);
            //options.AbsoluteExpiration = DateTime.Now.AddMinutes(1);

            // mempory key-value mory doldurursa framework silme sırasını nasıl ayarlar ? öncelik sırasına bakar. önem sırası
            //options.Priority = CacheItemPriority.Low;
            //options.Priority = CacheItemPriority.Normal;
            //options.Priority = CacheItemPriority.High;
            //options.Priority = CacheItemPriority.NeverRemove; // hepsini u seçersek ve mem dolarsa exception fırlatılır.
            options.Priority = CacheItemPriority.High;

            options.RegisterPostEvictionCallback((key, value, reason, state) => 
            {
                _memoryCache.Set<string>("callback", $"{key} : {value}, reason : {reason}, state : {state}");
            });

            _memoryCache.Set<string>("time", DateTime.Now.ToString(), options);

            //
            Product p = new () { Id = 1, Name = "kalem", Price = 11 };
            _memoryCache.Set<Product>("product:1", p);


            return View();
        }
        public IActionResult Show()
        {
            _memoryCache.TryGetValue("time", out string timeCache);
            ViewBag.time = timeCache;

            _memoryCache.TryGetValue("callback", out string callbackCache);
            ViewBag.callback = callbackCache;

            _memoryCache.TryGetValue("product:1", out Product productCache);
            ViewBag.product = productCache;

            //ViewBag.time = _memoryCache.Get<string>("time");

            return View();
        }
    }
}
