using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Senparc.CO2NET.Extensions;
using Senparc.NeuChar.App.AppStore;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.Entities.Request;
using Senparc.NeuChar.Helpers;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MessageHandlers;

namespace SenparcCourse.Service
{
    /// <summary>
    /// 自定义MessageHandler
    /// 把MessageHandler作为基类，重写对应请求的处理方法
    /// </summary>
    public class CustomMessageHandler : MessageHandler<CustomMessageContext>  /*如果不需要自定义，可以直接使用：MessageHandler<DefaultMpMessageContext> */
    {
        /*
       * 重要提示：v1.5起，MessageHandler提供了一个DefaultResponseMessage的抽象方法，
       * DefaultResponseMessage必须在子类中重写，用于返回没有处理过的消息类型（也可以用于默认消息，如帮助信息等）；
       * 其中所有原OnXX的抽象方法已经都改为虚方法，可以不必每个都重写。若不重写，默认返回DefaultResponseMessage方法中的结果。
       */


        public CustomMessageHandler(Stream inputStream, PostModel postModel, int maxRecordCount = 0, bool onlyAllowEncryptMessage = false, DeveloperInfo developerInfo = null) : base(inputStream, postModel, maxRecordCount, onlyAllowEncryptMessage, developerInfo)
        {
            base.GetCurrentMessageContext().Result.ExpireMinutes = 10;
        }

        public CustomMessageHandler(XDocument requestDocument, PostModel postModel, int maxRecordCount = 0, bool onlyAllowEncryptMessage = false, DeveloperInfo developerInfo = null) : base(requestDocument, postModel, maxRecordCount, onlyAllowEncryptMessage, developerInfo)
        {
        }

        public CustomMessageHandler(RequestMessageBase requestMessageBase, PostModel postModel, int maxRecordCount = 0, bool onlyAllowEncryptMessage = false, DeveloperInfo developerInfo = null) : base(requestMessageBase, postModel, maxRecordCount, onlyAllowEncryptMessage, developerInfo)
        {
        }

        /// <summary>
        /// 处理文字消息
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            //关键字回复
            var keyWordHandler = requestMessage.StartHandler(false)
                .Keyword("cmd", () =>
                {
                    var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
                    var storageModel = new StorageModel()
                    {
                        IsInCmd = true
                    };

                    base.GetCurrentMessageContext().Result.StorageData = storageModel;

                    responseMessage.Content = "您进入了CDM模式";

                    return responseMessage;
                }).Keywords(new string[] { "exit", "quit", "close" }, () =>
                  {
                      var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
                      var storageModel = base.GetCurrentMessageContext().Result.StorageData as StorageModel;


                      if (storageModel != null)
                      {
                          storageModel.IsInCmd = false;
                      }

                      responseMessage.Content = "您退出了CMD模式";

                      return responseMessage;
                  }).Regex(@"^http", () =>
                {
                    var responseMessage = this.CreateResponseMessage<ResponseMessageNews>();

                    var article = new Article
                    {
                        Description = "你看看这里\r\n有换行\r\n再换行",
                        PicUrl = "https://sdk.weixin.senparc.com/images/book-cover-front-small-3d-transparent.png",
                        Title = "您输入了：" + requestMessage.Content,
                        Url = "https://sdk.weixin.senparc.com/"
                    };

                    responseMessage.Articles.Add(article);

                    return responseMessage;
                }).Keyword("123", () =>
                {
                    var responseMessage = this.CreateResponseMessage<ResponseMessageText>();

                    responseMessage.Content = "关键字回复：" + requestMessage.Content;

                    return responseMessage;

                }).Default(() =>
                {
                    var responseMessage = this.CreateResponseMessage<ResponseMessageText>();

                    responseMessage.Content = "您输入了文字：" + requestMessage.Content;

                    return responseMessage;
                });


            return keyWordHandler.ResponseMessage;
        }

        /// <summary>
        /// 用户地理位置
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnLocationRequest(RequestMessageLocation requestMessage)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "您发送了地理位置信息：Lat-{0}，Lon-{1}".FormatWith(requestMessage.Location_X, requestMessage.Location_Y);

            return responseMessage;
        }

        /// <summary>
        /// 发送图片，返回图片
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnImageRequest(RequestMessageImage requestMessage)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageImage>();
            responseMessage.Image.MediaId = requestMessage.MediaId;
            return responseMessage;
        }

        /// <summary>
        /// 文字消息或EventKey进行统一的处理
        /// 会覆盖OnText 和 OnEvent 相同的处理方法
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnTextOrEventRequest(RequestMessageText requestMessage)
        {
            var textOrEventKey = requestMessage.Content;

            if (textOrEventKey == "123")
            {
                var responseMessage = this.CreateResponseMessage<ResponseMessageText>();

                responseMessage.Content = "这里是OnTextOrEventRequest的处理：" + requestMessage.Content;

                return responseMessage;
            }

            return base.OnTextOrEventRequest(requestMessage);
        }

        /// <summary>
        /// 单击事件
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_ClickRequest(RequestMessageEvent_Click requestMessage)
        {
            if (requestMessage.EventKey == "123")
            {
                var responseMessage = this.CreateResponseMessage<ResponseMessageNews>();

                var article = new Article
                {
                    Description = "你看看这里\r\n有换行\r\n再换行",
                    PicUrl = "https://sdk.weixin.senparc.com/images/book-cover-front-small-3d-transparent.png",
                    Title = "您点击了：" + requestMessage.EventKey,
                    Url = "https://sdk.weixin.senparc.com/"
                };

                responseMessage.Articles.Add(article);

                return responseMessage;
            }

            else if (requestMessage.EventKey == "no")
            {
                //不返回任何消息
                return new ResponseMessageNoResponse();
            }

            else if (requestMessage.EventKey == "A")
            {
                //检查当前消息上下文是否在Cmd状态下
                var responseMessage = this.CreateResponseMessage<ResponseMessageText>();

                var storageModel = base.GetCurrentMessageContext().Result.StorageData as StorageModel;

                if (storageModel != null)
                {
                    if (storageModel.IsInCmd)
                    {
                        var cmdCount = storageModel.CmdCount;
                        responseMessage.Content = "您当前在CMD模式下\r\nCmd Count: " + cmdCount + "\r\n上一条信息类型是：" + base.GetCurrentMessageContext().Result.RequestMessages.Last().MsgType;
                    }
                    else
                    {
                        responseMessage.Content = "您当前退出了CMD模式";
                    }
                }
                else
                {
                    responseMessage.Content = "您未曾登陆CMD";
                }

                return responseMessage;
            }

            else
            {
                var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
                responseMessage.Content = "您点击了：" + requestMessage.EventKey;

                return responseMessage;
            }
        }

        /// <summary>
        /// 在OnText\OnEvent之前执行
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task OnExecutingAsync(CancellationToken cancellationToken)
        {
            var storageModel = base.GetCurrentMessageContext().Result.StorageData as StorageModel;

            if (storageModel != null)
            {
                if (storageModel.IsInCmd)
                {
                    storageModel.CmdCount++;
                }

                if (storageModel.CmdCount > 5)
                {
                    //var responseMessageText = RequestMessage.CreateResponseMessage<ResponseMessageText>();
                    //responseMessageText.Content = "CMD Count 大于 5"; 
                    //ResponseMessage = responseMessageText;

                    base.CancelExcute = true;//取消以后所有的消息回复
                }
            }
            return base.OnExecutingAsync(cancellationToken);
        }

        /// <summary>
        /// 在OnText\OnEvent 之后执行
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task OnExecutedAsync(CancellationToken cancellationToken)
        {
            if (ResponseMessage is ResponseMessageText)
            {
                if (ResponseMessage != null)
                {
                    (ResponseMessage as ResponseMessageText).Content += "【消息签名】";
                } 
            }

            return base.OnExecutedAsync(cancellationToken);
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "当前服务器时间：" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");

            return responseMessage;
        }
    }
}
