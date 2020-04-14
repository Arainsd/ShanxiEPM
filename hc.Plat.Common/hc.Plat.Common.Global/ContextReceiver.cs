using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Dispatcher;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.IO;
using System.Runtime.Serialization;

namespace hc.Plat.Common.Global
{
    public class ContextReceiver: ICallContextInitializer
    {
        public void AfterInvoke(object correlationState)
        {
        }

        //public object BeforeInvoke(InstanceContext instanceContext, IClientChannel channel, Message message)
        //{
        //    object musername="aaaa";
        //    Console.Write("start.............");
        //    //message.Properties.TryGetValue("__Department", out musername);
        //    MessageBuffer mb = message.CreateBufferedCopy(65536);
        //    FileStream stream = new FileStream("c:\\log.xml", FileMode.Append);
        //    mb.WriteMessage(stream);
        //    stream.Flush();
        //    HttpRequestMessageProperty requestProperty=null;
        //    try
        //    {
                
        //        requestProperty = (HttpRequestMessageProperty)message.Properties[HttpRequestMessageProperty.Name];
        //    }
        //    catch (Exception e)
        //    {
        //        Console.Write("erris:"+e.Message);
        //    }
        //    foreach (string key in requestProperty.Headers.Keys)
        //    {
        //        if (key.StartsWith("__"))
        //        {
        //            ApplicationContext.Current[key] = requestProperty.Headers[key];
        //            Console.Write("key: {0}\nvalue: {1}\n", key, requestProperty.Headers[key]);
        //        }
        //    }
        //    Console.Write("end.............department="+musername);
        //    return null;
        //}

        public object BeforeInvoke(InstanceContext instanceContext, IClientChannel channel, Message message)
        {
           // Console.Write("start.............");
            ApplicationContext context = message.Headers.GetHeader<ApplicationContext>(ApplicationContext.ContextHeaderLocalName, ApplicationContext.ContextHeaderNamespace);
            if (context == null)
            {
                Console.Write("erris:无插入头信息" );
                return null;
            }

            ApplicationContext.Current = context;
           // Console.Write("ok:有头信息");
            return ApplicationContext.Current;
        }
    }
}
