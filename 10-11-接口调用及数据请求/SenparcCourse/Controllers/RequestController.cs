using System.Collections.Generic;
using System.Net;
using Senparc.CO2NET.HttpUtility;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http.Results;
using System.Web.Mvc;
using Newtonsoft.Json;
using Senparc.CO2NET.Extensions;
using SenparcCourse.Service.Class10;

namespace SenparcCourse.Controllers
{
    public class RequestController : Controller
    {
        // GET: Request
        /// <summary>
        /// 请求URL地址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public ActionResult Get(string url = "https://baidu.com")
        {
            var html = Senparc.CO2NET.HttpUtility.RequestUtility.HttpGet(null, url, encoding: Encoding.UTF8);

            html += "<script>alert('This is HttpGet From CO2NET ');</script>";

            return Content(html);
        }

        /// <summary>
        /// Cookie存储在CookieContainer中，可给后续的访问提供参数
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public ActionResult SimulateLogin(string url = "https://baidu.com")
        {
            CookieContainer cookieContainer = new CookieContainer();

            var html = Senparc.CO2NET.HttpUtility.RequestUtility.HttpGet(null, url, cookieContainer, encoding: Encoding.UTF8);

            return Content(html);
        }

        /// <summary>
        /// 发送POST请求消息
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public ActionResult Post(string url = "https://sdk.weixin.senparc.com/AsyncMethods/TemplateMessageTest")
        {
            Dictionary<string, string> formData = new Dictionary<string, string>();
            formData["checkcode"] = "318";

            var html = Senparc.CO2NET.HttpUtility.RequestUtility.HttpPost(null, url, null, formData, encoding: Encoding.UTF8);


            html += "<span style='color:red'>This is Post Request</span>";


            return Content(html);
        }

        /// <summary>
        /// 调用API返回的Json对象，转化为对应的实体类
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetJsonResult(string url = "http://t.weather.itboy.net/api/weather/city/101030100")
        {
            var cityCode = Regex.Match("天气 101030100", @"(?<=天气 )(\S+)").Value;


            var weather = Senparc.CO2NET.HttpUtility.Get.GetJson<Weather>(null, url);

            return weather.ToJson();
        }
    }
}