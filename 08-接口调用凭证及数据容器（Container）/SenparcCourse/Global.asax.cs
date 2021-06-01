using Senparc.CO2NET;
using Senparc.CO2NET.Cache;
using Senparc.CO2NET.RegisterServices;
using Senparc.CO2NET.Utilities;
using Senparc.Weixin.Entities;
using Senparc.Weixin.Exceptions;
using System.IO;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Senparc.Weixin;
using Senparc.Weixin.MP;


namespace SenparcCourse
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas(); 
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

             
            //设置全局 Debug 状态
            var isGLobalDebug = true;
            //全局设置参数，将被储存到 Senparc.CO2NET.Config.SenparcSetting
            var senparcSetting = SenparcSetting.BuildFromWebConfig(isGLobalDebug);
            //也可以通过这种方法在程序任意位置设置全局 Debug 状态：
            //Senparc.CO2NET.Config.IsDebug = isGLobalDebug;


            //CO2NET 全局注册，必须！！
            IRegisterService register = RegisterService.Start(senparcSetting).UseSenparcGlobal();
             

            #region 配置和使用 Redis          -- DPBMARK Redis

            //配置全局使用Redis缓存（按需，独立）
            var redisConfigurationStr = senparcSetting.Cache_Redis_Configuration;
            var useRedis = !string.IsNullOrEmpty(redisConfigurationStr) && redisConfigurationStr != "Redis配置";
            if (useRedis)//这里为了方便不同环境的开发者进行配置，做成了判断的方式，实际开发环境一般是确定的，这里的if条件可以忽略
            { 
                Senparc.CO2NET.Cache.Redis.Register.SetConfigurationOption(redisConfigurationStr);
                 
                Senparc.CO2NET.Cache.Redis.Register.UseKeyValueRedisNow();//键值对缓存策略（推荐）
                 
            }
             
            #endregion                        // DPBMARK_END
              

            /* 微信配置开始
             * 建议按照以下顺序进行注册
             */

            //设置微信 Debug 状态
            var isWeixinDebug = true;
            //全局设置参数，将被储存到 Senparc.Weixin.Config.SenparcWeixinSetting
            var senparcWeixinSetting = SenparcWeixinSetting.BuildFromWebConfig(isWeixinDebug);
            //也可以通过这种方法在程序任意位置设置微信的 Debug 状态：
            //Senparc.Weixin.Config.IsDebug = isWeixinDebug;

            //微信全局注册，必须！！
            register.UseSenparcWeixin(senparcWeixinSetting, senparcSetting)
                //注册公众号（可注册多个）                                                -- DPBMARK MP
                .RegisterMpAccount(senparcWeixinSetting, "【LY】测试公众号")// DPBMARK_END
                 
            ;

            /* 微信配置结束 */
        }  
         
    }
}
