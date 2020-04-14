using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    /// <summary>
    /// web URL配置项
    /// </summary>
    [Serializable]
    public class WebURLSettingView
    {
        public string Name { get; set; }

        public string URL { get; set; }
    }
}
