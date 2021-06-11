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

             
            //����ȫ�� Debug ״̬
            var isGLobalDebug = true;
            //ȫ�����ò������������浽 Senparc.CO2NET.Config.SenparcSetting
            var senparcSetting = SenparcSetting.BuildFromWebConfig(isGLobalDebug);
            //Ҳ����ͨ�����ַ����ڳ�������λ������ȫ�� Debug ״̬��
            //Senparc.CO2NET.Config.IsDebug = isGLobalDebug;


            //CO2NET ȫ��ע�ᣬ���룡��
            IRegisterService register = RegisterService.Start(senparcSetting).UseSenparcGlobal();
             

            #region ���ú�ʹ�� Redis          -- DPBMARK Redis

            //����ȫ��ʹ��Redis���棨���裬������
            var redisConfigurationStr = senparcSetting.Cache_Redis_Configuration;
            var useRedis = !string.IsNullOrEmpty(redisConfigurationStr) && redisConfigurationStr != "Redis����";
            if (useRedis)//����Ϊ�˷��㲻ͬ�����Ŀ����߽������ã��������жϵķ�ʽ��ʵ�ʿ�������һ����ȷ���ģ������if�������Ժ���
            { 
                Senparc.CO2NET.Cache.Redis.Register.SetConfigurationOption(redisConfigurationStr);
                 
                Senparc.CO2NET.Cache.Redis.Register.UseKeyValueRedisNow();//��ֵ�Ի�����ԣ��Ƽ���
                 
            }
             
            #endregion                        // DPBMARK_END
              

            /* ΢�����ÿ�ʼ
             * ���鰴������˳�����ע��
             */

            //����΢�� Debug ״̬
            var isWeixinDebug = true;
            //ȫ�����ò������������浽 Senparc.Weixin.Config.SenparcWeixinSetting
            var senparcWeixinSetting = SenparcWeixinSetting.BuildFromWebConfig(isWeixinDebug);
            //Ҳ����ͨ�����ַ����ڳ�������λ������΢�ŵ� Debug ״̬��
            //Senparc.Weixin.Config.IsDebug = isWeixinDebug;

            //΢��ȫ��ע�ᣬ���룡��
            register.UseSenparcWeixin(senparcWeixinSetting, senparcSetting)
                //ע�ṫ�ںţ���ע������                                                -- DPBMARK MP
                .RegisterMpAccount(senparcWeixinSetting, "��LY�����Թ��ں�")// DPBMARK_END
                 
            ;

            /* ΢�����ý��� */
        }  
         
    }
}
