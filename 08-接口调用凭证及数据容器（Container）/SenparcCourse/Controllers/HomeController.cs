using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Senparc.CO2NET.Cache;
using Senparc.CO2NET.Extensions;
using Senparc.Weixin.MP.Containers;

namespace SenparcCourse.Controllers
{
    public class HomeController : Controller
    {
        public static readonly string AppId = ConfigurationManager.AppSettings["WeixinAppId"];// Config.SenparcWeixinSetting.WeixinAppId;//与微信公众账号后台的AppId设置保持一致，区分大小写。

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

        public string CustomMessage(string openId = "oifDGvmdSfltOJPL2QSuCdEIN0io")
        {
            //获取AccessToken
            var accessToken = Senparc.Weixin.MP.Containers.AccessTokenContainer.GetAccessToken(AppId);

            //获取微信用户信息
            var userInfo = Senparc.Weixin.MP.AdvancedAPIs.UserApi.Info(AppId, openId);
            string strNickName = userInfo.nickname;
            string strGender = userInfo.sex == 1 ? "先生" : "女士";

            //推送客服消息,给用户
            Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendText(AppId, openId, "你好，{0} {1}".FormatWith(strNickName, strGender));

            for (int i = 0; i < 3; i++)
            {
                Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendText(AppId, openId, "这是1条来自服务器的客服消息." + (3 - i));
            }

            var result = Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendText(AppId, openId, "客服消息发送完毕.\r\n<a href=\"https://sdk.weixin.senparc.com/\">点击打开SDK官网</a>\r\n" + DateTime.Now);

            return result.ToJson();
        }

        public Task<string> CustomMessageAsync(string openId = "oifDGvmdSfltOJPL2QSuCdEIN0io")
        {
            return Task.Factory.StartNew(async () =>
           {
               for (int i = 0; i < 4; i++)
               {
                   await Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendTextAsync(AppId, openId, "这是1条来自服务器的客服消息.序号：{0}，时间：{1}".FormatWith((i + 1), DateTime.Now.ToString("HH:mm:ss.fff")));
               }

               return "异步消息发送成功";

           }).Result;


        }
    }
}