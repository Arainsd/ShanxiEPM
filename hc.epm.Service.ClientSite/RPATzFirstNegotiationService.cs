using hc.epm.Common;
using hc.epm.DataModel.Business;
using hc.epm.Service.Base;
using hc.epm.ViewModel;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.Service.ClientSite
{
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddRPAFirstNegotiation(RPA_TzFirstNegotiation model)
        {
            Result<int> result = new Result<int>();
            try
            {
                int rows = 0;
                RPA_TzFirstNegotiation first = new RPA_TzFirstNegotiation();
                SetCreateUser(first);
                SetCurrentUser(first);
                first.TalkTime = model.TalkTime;
                first.TalkAdress = model.TalkAdress;
                first.Fees = model.Fees;
                first.FeesTime = model.FeesTime;
                first.OurNegotiators = model.OurNegotiators;
                first.OtherNegotiators = model.OtherNegotiators;
                first.TalkResultName = model.TalkResultName;
                first.Remark = model.Remark;
                first.WriteMark = model.WriteMark;
                first.WriteResult = model.WriteResult;
                first.FollowOperate = model.FollowOperate;
                first.username = model.username;
                first.companys = model.companys;
                first.FilePath = model.FilePath;

                rows = DataOperateBusiness<RPA_TzFirstNegotiation>.Get().Add(first);
                DataOperateBusiness<RPA_TzFirstNegotiation>.Get().Add(first);
                //上传附件
                //  AddFilesBytzTable(first, model.TzAttachs);

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzFirstNegotiation.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTzFirstNegotiation");
            }
            return result;
        }
      
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteRPAFirstNegotiationByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<RPA_TzFirstNegotiation>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<RPA_TzFirstNegotiation>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteRPAFirstNegotiationByIds");
            }
            return result;
        }
       
    }
}
