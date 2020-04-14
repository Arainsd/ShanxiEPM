using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;

namespace XTInfos.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// 测试环境webservice引用
        /// </summary>
        XTDataInfo.HrmServicePortTypeClient client = new XTDataInfo.HrmServicePortTypeClient();
        XTEquipmentService.EquipmentServicePortTypeClient XTEquipmentCllient = new XTEquipmentService.EquipmentServicePortTypeClient();
        //本地固定访问IP
        public static string UrlIp = "10.202.6.229";
        #region 测试生成路径内容
        public static string FileNameUrlcompanyInfo = @"D:\数据备份\CompanyInfo.txt";
        public static string FileNameUrlDepartmentInfo = @"D:\数据备份\DepartmentInfo.txt";
        public static string FileNameUrlJobTitleInfo = @"D:\数据备份\JobTitleInfo.txt";
        public static string FileNameUrlUserInfo = @"D:\数据备份\UserInfo.txt";
        public static string FileNameUrlcheckUser = @"D:\数据备份\checkUser.txt";
        public static string FileNameUrlUserPicInfo = @"D:\数据备份\UserPicInfo.txt";
        public static string FileNameUrlChangeDetailInfo = @"D:\数据备份\ChangeDetailInfo.txt";
        public static string FileNameUrlSubCompanyInfo = @"D:\数据备份\SubCompanyInfo.txt";
        public static string FileNameUrlDepartmentInfoById = @"D:\数据备份\DepartmentInfoById.txt";
        public static string FileNameUrlJobTitleInfoById = @"D:\数据备份\JobTitleInfoById.txt";
        public static string FileNameUrlUserInfoById = @"D:\数据备份\UserInfoById.txt";
        public static string FileNameUrlEquipmentType = @"D:\数据备份\EquipmentType.txt";
        public static string FileNameUrlEquipmentInfo = @"D:\数据备份\EquipmentInfo.txt";
        #endregion
        public ActionResult Index()
        {
            #region 1获取所有公司信息列表

            try
            {
                var companyInfo = client.getHrmSubcompanyInfo(UrlIp);
                var companyInfos = JsonConvert.SerializeObject(companyInfo);
                System.IO.File.WriteAllText(FileNameUrlcompanyInfo, companyInfos, System.Text.Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            #endregion
            return View();
        }


        private void Xss()
        {
            #region 1获取所有公司信息列表
            var companyInfo = client.getHrmSubcompanyInfo(UrlIp);

            //if(companyInfo != null && companyInfo.Any())
            //{
            //    foreach (XTDataInfo.SubCompanyBean item in companyInfo)
            //    {
            //        long companyId = 0;
            //        if(long.TryParse(item._subcompanyid, out companyId))
            //        {
            //            var data = DataOperateBasic<Base_Company>.Get().GetModel(companyId);
            //            if(data == null)
            //            {
            //                // 插入
            //                data = new Base_Company();
            //                data.Id = companyId;
            //                // ...
            //            }
            //            else
            //            {
            //                // 修改
            //            }
            //            // 同步日志
            //        }
            //        else
            //        {
            //            // 错误记录
            //        }
            //    }
            //}


            var companyInfos = JsonConvert.SerializeObject(companyInfo);
            System.IO.File.WriteAllText(FileNameUrlcompanyInfo, companyInfos, System.Text.Encoding.UTF8);
            #endregion

            #region 2根据参数条件获取部门信息列表
            //50高速路分公司
            //141退出实职人员
            //181交流干部
            //var DepartmentInfo = client.getHrmDepartmentInfo(UrlIp, "181,141,50");
            //var DepartmentInfos = JsonConvert.SerializeObject(DepartmentInfo);
            //System.IO.File.WriteAllText(FileNameUrlDepartmentInfo, DepartmentInfos, System.Text.Encoding.UTF8);
            #endregion

            #region 3根据参数条件获取岗位信息列表
            //1965客户服务中心
            //565经营部机关
            //var JobTitleInfo = client.getHrmJobTitleInfo(UrlIp, "181,141,50", "1965,565");
            //var JobTitleInfos = JsonConvert.SerializeObject(JobTitleInfo);
            //System.IO.File.WriteAllText(FileNameUrlJobTitleInfo, JobTitleInfos, System.Text.Encoding.UTF8);
            #endregion

            #region 4根据参数条件获取用户信息列表
            //4288客户服务中心主任
            //4285客户经理
            //var UserInfo = client.getHrmUserInfo(UrlIp, "27", "", "", "", "");
            //var UserInfos = JsonConvert.SerializeObject(UserInfo);
            //System.IO.File.WriteAllText(FileNameUrlUserInfo, UserInfos, System.Text.Encoding.UTF8);
            #endregion

            #region 5检测用户
            //使用时用户密码需要解密
            //var checkUser = client.checkUser(UrlIp, "yangy-xs", "E921E574AA0651EAF32D56F8DF99AC03");
            //var checkUsers = JsonConvert.SerializeObject(checkUser);
            //System.IO.File.WriteAllText(FileNameUrlcheckUser, checkUsers, System.Text.Encoding.UTF8);
            #endregion

            #region 6获取用户照片
            //var UserPicInfo = client.getUserPic(UrlIp, "keaigyq1455");
            //var UserPicInfos = JsonConvert.SerializeObject(UserPicInfo);
            //System.IO.File.WriteAllText(FileNameUrlUserInfo, UserPicInfos, System.Text.Encoding.UTF8);
            #endregion

            #region 7根据参数条件获取变更明细列表
            //var ChangeDetailInfo = client.getChangeDetail(UrlIp, "4","2019-05-22");
            //var ChangeDetailInfos = JsonConvert.SerializeObject(ChangeDetailInfo);
            //System.IO.File.WriteAllText(FileNameUrlChangeDetailInfo, ChangeDetailInfos, System.Text.Encoding.UTF8);
            #endregion

            #region 8根据参数条件获取所有公司信息列表
            //var SubCompanyInfo = client.getHrmSubcompanyInfoById(UrlIp, "27");
            //var SubCompanyInfos = JsonConvert.SerializeObject(SubCompanyInfo);
            //System.IO.File.WriteAllText(FileNameUrlSubCompanyInfo, SubCompanyInfos, System.Text.Encoding.UTF8);
            #endregion

            #region 9根据参数条件获取部门信息列表
            //var DepartmentInfoById = client.getHrmDepartmentInfoById(UrlIp, "1965,565");
            //var DepartmentInfoByIds = JsonConvert.SerializeObject(DepartmentInfoById);
            //System.IO.File.WriteAllText(FileNameUrlDepartmentInfoById, DepartmentInfoByIds, System.Text.Encoding.UTF8);
            #endregion

            #region 10根据参数条件获取岗位信息列表
            //var JobTitleInfoById = client.getHrmJobTitleInfoById(UrlIp, "1285");
            //var JobTitleInfoByIds = JsonConvert.SerializeObject(JobTitleInfoById);
            //System.IO.File.WriteAllText(FileNameUrlJobTitleInfoById, JobTitleInfoByIds, System.Text.Encoding.UTF8);
            #endregion

            #region 11根据参数条件获取用户信息列表
            //var UserInfoById = client.getHrmUserInfoById(UrlIp, "30,49,136");
            //var UserInfoByIds = JsonConvert.SerializeObject(UserInfoById);
            //System.IO.File.WriteAllText(FileNameUrlUserInfoById, UserInfoByIds, System.Text.Encoding.UTF8);
            #endregion

            #region 12获取设备类型列表
            //var EquipmentType = XTEquipmentCllient.getEquipmentType(UrlIp);
            //var EquipmentTypes = JsonConvert.SerializeObject(EquipmentType);
            //System.IO.File.WriteAllText(FileNameUrlEquipmentType, EquipmentTypes, System.Text.Encoding.UTF8);
            #endregion

            #region 13根据参数条件获取设备列表
            //var EquipmentInfo = XTEquipmentCllient.getEquipmentInfo(UrlIp,"1");
            //var EquipmentInfos = JsonConvert.SerializeObject(EquipmentInfo);
            //System.IO.File.WriteAllText(FileNameUrlEquipmentInfo, EquipmentInfos, System.Text.Encoding.UTF8);
            #endregion
        }
    }
}