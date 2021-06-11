using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Senparc.CO2NET.MessageQueue;
using Senparc.NeuChar.Entities;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.Entities;

namespace SenparcCourse.Service
{
    public class MessageQueueHandler
    {
        public static readonly string AppId = ConfigurationManager.AppSettings["WeixinAppId"];// Config.SenparcWeixinSetting.WeixinAppId;//与微信公众账号后台的AppId设置保持一致，区分大小写。


        public IResponseMessageBase SendMessage(string openId, IResponseMessageBase ResponseMessage)
        {
            var senparcMessageQueue = new SenparcMessageQueue();

            if (ResponseMessage is ResponseMessageText)
            {
                string strResponseMsg = "";

                //以下是：把同一用户的多个消息内容，加入到队列中

                //myKey 加入队列排序执行 
                var myKey = SenparcMessageQueue.GenerateKey("MessageQueueHandlerAsync", ResponseMessage.GetType(),
                    Guid.NewGuid().ToString(), "SendMessage");

                senparcMessageQueue.Add(myKey, () =>
                {
                    //把需要回复的消息，整理一下
                    var asyncResponseMessage = ResponseMessage as ResponseMessageText;
                    if (asyncResponseMessage != null)
                    {
                        asyncResponseMessage.Content += "\r\n【1-执行超过5秒，以客服消息回复】";

                        strResponseMsg = asyncResponseMessage.Content;
                    }
                });

                //Thread.Sleep(10); 

                //myKey2 加入队列排序执行 
                var myKey2 = SenparcMessageQueue.GenerateKey("MessageQueueHandlerAsync", ResponseMessage.GetType(),
                    Guid.NewGuid().ToString(), "SendMessage");
                senparcMessageQueue.Add(myKey2, () =>
                {
                    //把需要回复的消息，整理一下
                    var asyncResponseMessage = ResponseMessage as ResponseMessageText;
                    if (asyncResponseMessage != null)
                    {
                        asyncResponseMessage.Content += "\r\n【2-执行超过5秒，以客服消息回复】";

                        strResponseMsg = asyncResponseMessage.Content;
                    }
                });
                
                //执行队列，把需要回复的消息整理
                SenparcMessageQueue.OperateQueue();

                //通过客服消息的方式，下发给用户
                CustomApi.SendText(AppId, openId, strResponseMsg);


                //不再给用户回复消息  
                return new ResponseMessageNoResponse();
            }
            return ResponseMessage;
        }

    }
}
