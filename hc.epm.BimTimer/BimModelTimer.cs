using hc.epm.Common;
using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.BimTimer
{
    public class BimModelTimer
    {
        /// <summary>
        /// 轻量化BIM模型
        /// </summary>
        /// <returns></returns>
        public void TimerSetBimModel()
        {
            string path = ConfigurationManager.AppSettings["BIMPathUrl"];

            DirectoryInfo dinfo = new DirectoryInfo(path);
            string[] temp = null;
            if (dinfo.Exists)
            {
                FileInfo[] finfo = dinfo.GetFiles();
                temp = new string[finfo.Length];
                for (int i = 0; i < finfo.Length; i++)
                {
                    temp[i] = finfo[i].Name.Substring(finfo[i].Name.LastIndexOf("\\") + 1, (finfo[i].Name.LastIndexOf(".") - finfo[i].Name.LastIndexOf("\\") - 1));
                }

                string str = String.Join(",", temp);
                var bimList = DataOperateBusiness<Epm_Bim>.Get().GetList(t => (str.Contains(t.BIMName))).ToList();
                if (bimList.Count > 0)
                {
                    //修改模型状态
                    foreach (var item in bimList)
                    {
                        item.BIMState = BIMModelState.BIMLightWeightSuccess.ToString();
                        item.BIMAddress = "/Tools/output/" + item.BIMName + "/" + item.BIMName + "List.json";
                        item.BIMDBPath = "/Tools/output/" + item.BIMName + ".db";
                    }
                    var rows = DataOperateBusiness<Epm_Bim>.Get().UpdateRange(bimList);
                }
            }
        }
    }
}
