using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
   public class TzConDrawingView
    {
        //项目批复信息
        public Epm_TzProjectApprovalInfo TzProjectApproval { get; set; }
        //施工图纸信息
        public Epm_TzConDrawing TzConDrawing { get; set; }

        public TzConDrawingView()
        {
            TzProjectApproval = new Epm_TzProjectApprovalInfo();
            TzConDrawing = new Epm_TzConDrawing();
        }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string StationName { get; set; }

        /// <summary>
        /// 站库名称
        /// </summary>
        public string ProjectName { get; set; }

        public string NatureName { get; set; }
        public string ProjectCode { get; set; }
        public string CompanyName { get; set; }
        public decimal? PredictMoney { get; set; }
    }
}
