using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class ProjectBasicsInfoView
    {
        //id
        public long Id { get; set; }
        //站库名称
        public string StationName { get; set; }

        //项目性质
        public string NatureName { get; set; }

        //项目类型
       // public string  { get; set; }

       //项目编码
        public string ProjectCode { get; set; }
        //地区公司
        // public string  { get; set; }
        //地市公司
        public string CompanyName { get; set; }

        //估算投资
        public decimal? PredictMoney { get; set; }

        //工程费用
        public decimal? EngineeringCost { get; set; }
        //土地费用
        public decimal? LandCosts { get; set; }
        //其它费用
        public decimal? OtherExpenses { get; set; }
        //地区公司
        public string RegionCompany { get; set; }
        //项目类型
        public string ProjectType { get; set; }
    }
}
