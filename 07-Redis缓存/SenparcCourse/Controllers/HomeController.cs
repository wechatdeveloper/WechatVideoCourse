using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Senparc.CO2NET.Cache;

namespace SenparcCourse.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        //Static 保存在内存中，页面观察数据变化
        private static int _count;

        public static int Count
        {
            get { return _count; }

            set
            {
                _count = value;
            }
        }

        public ContentResult LockTest()
        {
            //获取当前缓存实例
            var strategy = CacheStrategyFactory.GetObjectCacheStrategyInstance();
            using (strategy.BeginCacheLock("HomeController", "LockTest"))
            {
                Thread.Sleep(300);
                Count++;
                return Content("Count:" + Count);
            }
        }
    }
}