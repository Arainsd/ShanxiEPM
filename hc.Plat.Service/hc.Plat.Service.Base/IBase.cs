using System.ServiceModel; 
using hc.Plat.Common.Global;

namespace hc.Plat.Service.Base
{

    /// <summary>
    /// 服务基础类库接口；
    /// </summary>
    [ServiceContract]
    public interface IBase
    {
        /// <summary>
        /// 测试服务正常
        /// </summary>
        [OperationContract]
        Result<object> Test();
        
    }
}