using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class OperateProject
    {
        /// <summary>
        /// 项目
        /// </summary>
        public Epm_Project Epm_Project { get; set; }

        /// <summary>
        /// 总批复及构成
        /// </summary>
        public List<Epm_ProjectConstitute> Epm_ProjectConstitute { get; set; }

        /// <summary>
        /// 工程内容要点
        /// </summary>
        public List<Epm_ProjectWorkMainPoints> Epm_ProjectWorkMainPoints { get; set; }

        /////////////////////////////////////////////////

        /// <summary>
        /// 里程碑Id
        /// </summary>
        public List<long> milepostIdList { get; set; }

        /// <summary>
        /// 项目资料Id
        /// </summary>
        public List<long> dataFileIdList { get; set; }

        /// <summary>
        /// 第三方资料Id
        /// </summary>
        public List<long> companyList { get; set; }

        /// <summary>
        /// 第三方单位
        /// </summary>
        public  List<Epm_ProjectCompany> Epm_ProjectCompany { get; set; }
        
        /// <summary>
        /// 项目里程碑
        /// </summary>
        public List<Epm_ProjectMilepost> Epm_ProjectMilepost { get; set; }

        /// <summary>
        /// 项目关联资料
        /// </summary>
        public List<Epm_ProjectData> Epm_ProjectData { get; set; }

        /// <summary>
        /// 上传图片
        /// </summary>
        public List<Base_Files> fileListImg { get; set; }

        /// <summary>
        /// 上传附件
        /// </summary>
        public List<Base_Files> fileListFile { get; set; }

        /// <summary>
        /// 合同列表
        /// </summary>
        public List<Epm_Contract> Epm_ProjectContract { get; set; }

        /// <summary>
        /// 项目问题列表
        /// </summary>
        public List<Epm_Question> Epm_Question { get; set; }
    }
}
