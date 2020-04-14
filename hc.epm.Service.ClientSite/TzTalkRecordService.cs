using hc.epm.DataModel.Business;
using hc.epm.Service.Base;
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
        public Result<Epm_TzTalkRecord> GetTzTalkRecordItemModel(long id)
        {
            Result<Epm_TzTalkRecord> result = new Result<Epm_TzTalkRecord>();
            try
            {
                var model = DataOperateBusiness<Epm_TzTalkRecord>.Get().GetModel(id);

                List<Epm_TzAttachs> tzAttachsList = new List<Epm_TzAttachs>();
                tzAttachsList = GetFilesByTZTable("Epm_TzTalkRecord", id).Data;
                if (tzAttachsList != null && tzAttachsList.Any())
                {
                    model.TzAttachs = tzAttachsList;
                }
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzTalkRecordItemModel");
            }
            return result;
        }
    }
}
