using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.IO;

namespace hc.Plat.Common.Global
{
    public class ContextSender: IClientMessageInspector
    {
        public void AfterReceiveReply(ref Message reply, object correlationState) { }
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            MessageHeader<ApplicationContext> contextHeader = new MessageHeader<ApplicationContext>(ApplicationContext.Current);
            request.Headers.Add(contextHeader.GetUntypedHeader(ApplicationContext.ContextHeaderLocalName, ApplicationContext.ContextHeaderNamespace));

            /*

            string path = "c:\\log.txt";
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            //开始写入
            sw.Write("ok!!!!");
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
            //Console.Write("send.............");
            //string path="c:\\log.txt";
           
            //HttpRequestMessageProperty requestProperty;
            //if (!request.Properties.Keys.Contains(HttpRequestMessageProperty.Name))
            //{
            //    FileStream fs = new FileStream(path, FileMode.Create);
            //    StreamWriter sw = new StreamWriter(fs);
            //    //开始写入
            //    sw.Write("无httprequest!!!!");
            //    //清空缓冲区
            //    sw.Flush();
            //    //关闭流
            //    sw.Close();
            //    fs.Close();
            //    requestProperty = new HttpRequestMessageProperty();
            //    //---------2015-10-05----//
            //    request.Properties.Add(HttpRequestMessageProperty.Name, requestProperty);
            //}
            //else
            //{ 
            //    requestProperty = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
            //}
            //foreach(var context in ApplicationContext.Current)
            //{
            //    requestProperty.Headers.Add(context.Key, context.Value.ToString());
            //}

            */

            return null;
        }
    }
}
