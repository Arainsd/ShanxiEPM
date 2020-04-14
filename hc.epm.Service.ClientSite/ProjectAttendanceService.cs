using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.ViewModel;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.Service.ClientSite
{
    public partial class ClientSiteService
    {
        #region 分公司
        public Result<List<AttendanceBranchCountView>> GetBranchCount(string companyId, string startTime, string endTime, int pageIndex, int pageSize)
        {
            var Attenlist = DataOperateBusiness<Epm_ProjectAttendance>.Get().GetList();//获取考勤表信息
            string Attendancetype = string.Join(",", Attenlist.Select(t => t.AttendanceType).Distinct().ToList());//AttendanceType考勤人员类型

            //  var SignInformation = DataOperateBusiness<Epm_SignInformation>.Get().GetList();//获取考勤表信息

            Result<List<AttendanceBranchCountView>> result = new Result<List<AttendanceBranchCountView>>();

            var query = from a in context.Epm_Project.Where(p => p.IsDelete == false)
                        join b in context.Epm_SignInformation.Where(p => p.IsDelete == false) on a.Id equals b.projectId into temp
                        from tt in temp.DefaultIfEmpty()
                        select new
                        {
                            a,
                            tt,
                            OperateDate = DbFunctions.TruncateTime(tt.SignTime)
                        };
            int i = 1;

            List<AttendanceBranchCountView> listView = new List<AttendanceBranchCountView>();

            if (!string.IsNullOrEmpty(companyId))
            {
                query = query.Where(p => p.a.CompanyId.ToString() == companyId);
            }
            if (!string.IsNullOrEmpty(startTime))
            {
                DateTime stime = Convert.ToDateTime(startTime);
                query = query.Where(p => p.tt.SignTime >= stime);
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                DateTime stime = Convert.ToDateTime(endTime);
                query = query.Where(p => p.tt.SignTime <= stime);
            }
            var list = query.GroupBy(m => new { m.a.CompanyId, m.a.CompanyName, m.OperateDate }).Select(m => new AttendanceBranchCountView
            {
                sort = i + 1,
                CompanyId = m.Key.CompanyId.ToString(),
                CompanyName = m.Key.CompanyName,
                AttendanceCount = m.Count(),//总考勤人数
                dateTime = m.Key.OperateDate.ToString(),


            }).ToList();
            foreach (var item in list)
            {
                item.ProjectCount = GetProjectCount(item.CompanyId);//总项目数


                item.AttendanceCount = GetAllActualCount(item.CompanyId, Attendancetype);//公司总考勤人数-项目下的考勤人数

                item.ActualCount = GetActualCount(item.CompanyId, item.dateTime);//实际打卡次数

                item.RegulationsCount = item.AttendanceCount * 2;//总考勤次数（人数*规定打卡次数）

                if (item.ActualCount != 0 && item.RegulationsCount != 0)
                {
                    double percent = Math.Round(item.ActualCount * 1.00 / item.RegulationsCount * 100.0, 4);
                    item.AttendanceRate = percent.ToString() + "%";//得到5.8824%
                }
            }


            if (list.Count > 0)
            {
                result.AllRowsCount = list.Count;
                list = list.OrderByDescending(t => t.dateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                result.Data = list;
                result.Flag = EResultFlag.Success;
            }
            else
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
            }
            return result;
        }
        //公司总考勤人数=公司-项目-供应商-人员
        private int GetAllActualCount(string CompanyId, string postValue)
        {
            int ii = 0;

            var quer = (from a in context.Epm_ProjectCompany.Where(p => !p.IsDelete && p.CompanyId.ToString() == CompanyId)
                        select new
                        {
                            CompanyId = a.CompanyId,
                            CompanyName = a.CompanyName,
                        }).ToList();
            if (quer != null)
            {
                ii += DataOperateBasic<Base_User>.Get().GetList(p => p.CompanyId.ToString() == CompanyId).Count();//先忽略查询条件
                // ii += DataOperateBasic<Base_User>.Get().GetList(p => p.CompanyId.ToString() == CompanyId && postValue.Contains(p.PostValue)).Count();

            }
            return ii;
        }
        /// <summary>
        /// 总项目数-CompanyId 公司ID,项目对应的是分公司ID,统计所有的分公司
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        private int GetProjectCount(string CompanyId)
        {
            int ii = 0;

            var quer = (from a in context.Epm_Project.Where(p => !p.IsDelete)
                        select new
                        {
                            CompanyId = a.CompanyId,
                            CompanyName = a.CompanyName,
                        }).ToList();

            var strIds = quer.ToList();

            foreach (var item in strIds)
            {
                if (item.CompanyId.ToString() != null)
                {

                    if (item.CompanyId.ToString() == CompanyId)
                    {
                        ii++;
                    }
                }
            }
            return ii;

        }

        /// <summary>
        /// 实际打卡总数
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        private int GetActualCount(string CompanyId, string attenTime)
        {
            //在设置的最早时间段之前只要打卡成功，考勤次数 + 1；
            //在设置的最晚时间段之后打卡成功，考勤次数 + 1；
            //中间设置的时间段，用户只能在规定的时间段内打卡，不能提前也不能推迟，中间设置的时间段内存在打卡成功，不管用户在规定时间内打卡几次，考勤次数 + 1；
            int ii = 0;
            if (!string.IsNullOrEmpty(attenTime))
            {
                attenTime = attenTime.Substring(0, 11);
            }
            List<string> userIdList = new List<string>();
            int num = 0;
            int count = 0;
            var Attenlist = DataOperateBusiness<Epm_ProjectAttendance>.Get().GetList(p => p.IsDelete == false).OrderBy(p => p.StartTime).ToList();//获取每天考勤表信息

            num = DataOperateBusiness<Epm_ProjectAttendance>.Get().GetList(p => p.IsDelete == false).OrderBy(p => p.StartTime).GroupBy(p => p.StartTime).Count();

            var userAttenlist = DataOperateBusiness<Epm_SignInformation>.Get().GetList();//具体的打卡记录表

            var query = from ta in context.Epm_SignInformation.Where(p => p.IsDelete == false)
                        join p in context.Epm_Project.Where(p => p.CompanyId.ToString() == CompanyId) on ta.projectId equals p.Id
                        select new
                        {
                            ta.SignTime,//打卡时间
                            ta.projectId,//项目id
                            p.CompanyId,//单位id
                            ta.SignResult,//打卡状态
                            ta.userId,
                        };

            var strIds = query.ToList();//取出来该单位下所有的打卡记录
            if (num == 0)
            {
                return ii;
            }
            for (int i = 0; i <= num; i++)
            {
                foreach (var item in strIds)//所有的打卡记录
                {
                    if (item.CompanyId != null)//判断是不是有效的打卡记录
                    {
                        var StartTime = DateTime.Parse(attenTime + Attenlist[i].StartTime.ToString());
                        var EndTime = DateTime.Parse(attenTime + Attenlist[i].EndTime.ToString());

                        if (item.SignResult == "Success" && item.SignTime.ToString().Contains(attenTime))//打卡成功
                        {
                            if (count == 0)//第一个时间段
                            {
                                if (item.SignTime <= StartTime)
                                {
                                    ii++;
                                }
                            }
                            else if (count == num)//最后一个时间段
                            {
                                if (item.SignTime >= EndTime)
                                {
                                    ii++;
                                }
                            }
                            else//中间时间段
                            {
                                if (item.SignTime <= StartTime && item.SignTime >= EndTime)
                                {
                                    //判断是不是已经打卡过,没有+1
                                    var su = userIdList.Contains(item.userId.ToString());
                                    if (su)
                                    {
                                        userIdList.Add(item.userId.ToString());
                                        ii++;
                                    }
                                }
                            }
                        }
                    }
                }
                count++;
            }

            return ii;
        }
        #endregion

        #region 项目统计
        public Result<List<AttendanceBranchCountView>> GetBranchProjectCount(string name, string companyId, string startTime, string endTime, int pageIndex, int pageSize)
        {
            Result<List<AttendanceBranchCountView>> result = new Result<List<AttendanceBranchCountView>>();
            int num = DataOperateBusiness<Epm_ProjectAttendance>.Get().GetList(p => p.IsDelete == false).OrderBy(p => p.StartTime).GroupBy(p => p.StartTime).Count();


            var query = from a in context.Epm_Project.Where(p => p.IsDelete == false)
                        join b in context.Epm_SignInformation.Where(p => p.IsDelete == false) on a.Id equals b.projectId into temp
                        from tt in temp.DefaultIfEmpty()
                        select new
                        {
                            a,
                            tt,
                            OperateDate = DbFunctions.TruncateTime(tt.SignTime)
                        };
            if (!string.IsNullOrEmpty(companyId))
            {
                query = query.Where(p => p.a.CompanyId.ToString() == companyId);
            }
            if (!string.IsNullOrEmpty(startTime))
            {
                DateTime stime = Convert.ToDateTime(startTime);
                query = query.Where(p => p.tt.SignTime >= stime);
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                DateTime stime = Convert.ToDateTime(endTime);
                query = query.Where(p => p.tt.SignTime <= stime);
            }
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(p => p.a.Name.Contains(name));
            }
            List<AttendanceBranchCountView> listView = new List<AttendanceBranchCountView>();

            var list = query.GroupBy(m => new { m.a.CompanyId, m.a.CompanyName, m.OperateDate, m.a.Name, m.a.Id }).Select(m => new AttendanceBranchCountView
            {
                CompanyId = m.Key.CompanyId.ToString(),
                CompanyName = m.Key.CompanyName,
                AttendanceCount = m.Count(),//总考勤人数
                dateTime = m.Key.OperateDate.ToString(),
                Name = m.Key.Name,
                projectId = m.Key.Id.ToString(),
            }).ToList();

            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    AttendanceBranchCountView view = new AttendanceBranchCountView();
                    view.PreName = item.CompanyName ?? "";
                    view.Name = item.Name ?? "";//项目名字
                    view.dateTime = item.dateTime == null ? "" : item.dateTime == "" ? "" : item.dateTime.Substring(0, 11);

                    view.AttendanceCount = GetProjectAllActualCount(item.projectId.ToString());//总考勤人数(每个项目下考勤的人数)

                    view.RegulationsCount = view.AttendanceCount * num;//总考勤次数（人数*规定打卡次数）
                    view.ActualCount = GetProjectActualCount(item.projectId.ToString(), view.dateTime, num);//实际打卡总数

                    if (view.ActualCount != 0 && view.RegulationsCount != 0)
                    {
                        double percent = Math.Round(view.ActualCount * 1.00 / view.RegulationsCount * 100.0, 4);
                        view.AttendanceRate = percent.ToString() + "%";//得到5.8824%
                    }
                    listView.Add(view);

                }
            }

            if (list.Count > 0)
            {
                result.AllRowsCount = listView.Count;
                listView = listView.OrderByDescending(t => t.dateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                result.Data = listView;
                result.Flag = EResultFlag.Success;
            }
            else
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
            }

            return result;
        }
        /// <summary>
        /// 总考勤人数--考勤表-人员表--项目表？项目表跟考勤表的关系？
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        private int GetProjectAllActualCount(string projectId)//项目下工程供应商-总考勤的人数
        {
            int ii = 0;

            var quer = (from a in context.Epm_ProjectCompany.Where(p => !p.IsDelete && p.ProjectId.ToString() == projectId)
                        select new
                        {
                            CompanyId = a.CompanyId,
                            CompanyName = a.CompanyName,
                        }).ToList();
            if (quer != null)
            {
                foreach (var item in quer)
                {
                    ii += DataOperateBasic<Base_User>.Get().GetList(p => p.CompanyId == item.CompanyId).Count();//数据不全先忽略查询条件
                    // ii += DataOperateBasic<Base_User>.Get().GetList(p => p.CompanyId.ToString() == CompanyId && postValue.Contains(p.PostValue)).Count();
                }

            }
            return ii;
        }

        /// <summary>
        /// 项目实际打卡总数
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        private int GetProjectActualCount(string projectId, string attenTime, int num)
        {
            //在设置的最早时间段之前只要打卡成功，考勤次数 + 1；
            //在设置的最晚时间段之后打卡成功，考勤次数 + 1；
            //中间设置的时间段，用户只能在规定的时间段内打卡，不能提前也不能推迟，中间设置的时间段内存在打卡成功，不管用户在规定时间内打卡几次，考勤次数 + 1；

            int ii = 0;
            List<string> userIdList = new List<string>();
            int count = 0;
            if (num == 0 || string.IsNullOrEmpty(attenTime))
            {
                return ii;
            }
            var Attenlist = DataOperateBusiness<Epm_ProjectAttendance>.Get().GetList(p => p.IsDelete == false).OrderBy(p => p.StartTime).ToList();//获取每天考勤表信息

            var AttenTimelist = Attenlist.GroupBy(m => new { m.EndTime, m.StartTime }).Select(m => new ProjectAttendanceTimeView
            {
                EndTime = m.Key.EndTime.ToString(),
                StartTime = m.Key.StartTime,

            }).OrderByDescending(p => p.StartTime).ToList();
            // var att=from a in context.Epm_ProjectAttendance.Where(p => p.IsDelete == false)

            var query = from ta in context.Epm_Project.Where(p => p.IsDelete == false)
                        join p in context.Epm_SignInformation.Where(p => p.SignResult == "Success" && p.projectId.ToString() == projectId) on ta.Id equals p.projectId
                        select new
                        {
                            p.userId,
                            p.SignTime,
                        };

            var strIds = query.ToList();//得到该项目下当天的打卡信息

            for (int i = 0; i < AttenTimelist.Count; i++)
            {
                var StartTime = DateTime.Parse(attenTime + AttenTimelist[i].StartTime.ToString());
                var EndTime = DateTime.Parse(attenTime + AttenTimelist[i].EndTime.ToString());
                foreach (var item in strIds)//所有的打卡记录
                {
                    string time = item.SignTime.ToString("yyyy-MM-dd");
                    if (time != attenTime.TrimEnd())
                    {
                        continue;
                    }

                    if (count == 0)//第一个时间段
                    {
                        if (item.SignTime <= StartTime)
                        {
                            ii++;
                        }
                    }
                    else if (count == num)//最后一个时间段
                    {
                        if (item.SignTime >= EndTime)
                        {
                            ii++;
                        }
                    }
                    else//中间时间段
                    {
                        if (item.SignTime <= StartTime && item.SignTime >= EndTime)
                        {
                            //判断是不是已经打卡过,没有+1
                            var su = userIdList.Contains(item.userId.ToString());
                            if (!su)
                            {
                                userIdList.Add(item.userId.ToString());
                                ii++;
                            }
                        }
                    }

                }
                count++;
            }
            return ii;

        }
        #endregion

        #region 人员统计
        public Result<List<AttendanceBranchCountView>> GetBranchUserCount(string ProName, string companyId, string startTime, string endTime, string userName, int pageIndex, int pageSize)
        {
            Result<List<AttendanceBranchCountView>> result = new Result<List<AttendanceBranchCountView>>();

            var query = from a in context.Epm_Project.Where(p => p.IsDelete == false)
                        join b in context.Epm_SignInformation.Where(p => p.IsDelete == false) on a.Id equals b.projectId into temp
                        from tt in temp.DefaultIfEmpty()
                        select new
                        {
                            a,
                            tt,
                            OperateDate = DbFunctions.TruncateTime(tt.SignTime)
                        };
            // query = query.OrderByDescending(t => t.a.CompanyName).Where(t => string.IsNullOrEmpty(projectNature) || projectNature.Contains(t.a.ProjectNature));

            if (!string.IsNullOrEmpty(companyId))
            {
                query = query.Where(p => p.a.CompanyId.ToString() == companyId);
            }
            if (!string.IsNullOrEmpty(startTime))
            {
                DateTime stime = Convert.ToDateTime(startTime);
                query = query.Where(p => p.tt.SignTime >= stime);
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                DateTime stime = Convert.ToDateTime(endTime);
                query = query.Where(p => p.tt.SignTime <= stime);
            }
            if (!string.IsNullOrEmpty(ProName))
            {
                query = query.Where(p => p.a.Name.Contains(ProName));
            }
            if (!string.IsNullOrEmpty(userName))
            {
                query = query.Where(p => p.tt.userName.Contains(userName));
            }
            List<AttendanceBranchCountView> listView = new List<AttendanceBranchCountView>();
            var list = query.GroupBy(m => new { m.a.CompanyId, m.a.CompanyName, m.OperateDate, m.a.Name, m.a.Id, m.tt.userId, m.tt.userName }).Select(m => new AttendanceBranchCountView
            {
                //sort = i + 1,
                CompanyId = m.Key.CompanyId.ToString(),
                CompanyName = m.Key.CompanyName,
                AttendanceCount = m.Count(),//总考勤人数
                dateTime = m.Key.OperateDate.ToString(),
                Name = m.Key.Name,
                projectId = m.Key.Id.ToString(),
                userName = m.Key.userName,
                userID = m.Key.userId.ToString(),
            }).ToList();


            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    //if (item.a.Id.ToString()== "1211848000893227008" || item.a.Id.ToString() == "1211872754194845696")
                    //{
                    AttendanceBranchCountView view = new AttendanceBranchCountView();
                    view.PreName = item.CompanyName ?? "";//分公司
                    view.Name = item.Name ?? "";//项目名字
                    view.dateTime = item.dateTime == null ? "" : item.dateTime == "" ? "" : item.dateTime.Substring(0, 11);
                    view.userName = item.userName; //考勤人员
                    view.RegulationsCount = 1 * 2;// 总考勤次数（人数* 规定打卡次数）

                    view.ActualCount = GetUserAttendanceUserCount(item.projectId, item.userID, view.dateTime);//实际打卡总数

                    if (view.ActualCount != 0 && view.RegulationsCount != 0)
                    {
                        double percent = Math.Round(view.ActualCount * 1.00 / view.RegulationsCount * 100.0, 4);
                        view.AttendanceRate = percent.ToString() + "%";//得到5.8824%
                    }
                    listView.Add(view);
                    // }

                }
            }

            if (list.Count > 0)
            {
                result.AllRowsCount = listView.Count;
                listView = listView.OrderByDescending(t => t.dateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                result.Data = listView;
                result.Flag = EResultFlag.Success;
            }
            else
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
            }

            return result;
        }
        /// <summary>
        /// 总考勤人数--考勤表-人员表--项目表？项目表跟考勤表的关系？
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        //private int GetUserAttendanceCount(string CompanyId)
        //{
        //    int ii = 0;
        //    var query = (from a in basicContext.Base_Company.Where(p => p.IsDelete == false && p.Type == "Owner" && p.PId != 0)
        //                 join b in basicContext.Base_User.Where(p => p.IsDelete == false) on a.Id equals b.CompanyId into temp
        //                 from b in temp.DefaultIfEmpty()
        //                 select (new UserAndCompanyView { PId = a.PId, PreName = a.PreName, CompanyId = a.Id, UserID = b.Id, PostValue = b.PostValue })).ToList();//单位和人员信息


        //    foreach (var item in query)
        //    {
        //        if (item.CompanyId != null)
        //        {

        //            if (item.CompanyId.ToString() == CompanyId)
        //            {
        //                ii++;
        //            }
        //        }
        //    }
        //    return ii;

        //}

        /// <summary>
        /// 实际打卡总数
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        private int GetUserAttendanceUserCount(string projectId, string userid, string attenTime)
        {
            //在设置的最早时间段之前只要打卡成功，考勤次数 + 1；
            //在设置的最晚时间段之后打卡成功，考勤次数 + 1；
            //中间设置的时间段，用户只能在规定的时间段内打卡，不能提前也不能推迟，中间设置的时间段内存在打卡成功，不管用户在规定时间内打卡几次，考勤次数 + 1；

            int ii = 0;
            List<string> userIdList = new List<string>();
            int num = 0;
            int count = 0;
            var Attenlist = DataOperateBusiness<Epm_ProjectAttendance>.Get().GetList(p => p.IsDelete == false).OrderBy(p => p.StartTime).ToList();//获取每天考勤表信息

            num = DataOperateBusiness<Epm_ProjectAttendance>.Get().GetList(p => p.IsDelete == false).OrderBy(p => p.StartTime).GroupBy(p => p.StartTime).Count();
            if (num == 0 || string.IsNullOrEmpty(attenTime))
            {
                return ii;
            }

            var query = from ta in context.Epm_Project.Where(p => p.IsDelete == false)
                        join p in context.Epm_SignInformation.Where(p => p.SignResult == "Success") on ta.Id equals p.projectId
                        select new
                        {
                            ta.Id,
                            p.userId,
                            p.SignTime,
                            p.SignResult,
                        };
            if (!string.IsNullOrEmpty(attenTime))
            {
                attenTime = attenTime.Substring(0, 11);
            }
            if (!string.IsNullOrEmpty(userid))
            {
                query = query.Where(p => p.userId.ToString() == userid);
            }
            var strIds = query.Where(p => p.Id.ToString() == projectId).ToList();
            for (int i = 0; i <num; i++)
            {

                var StartTime = DateTime.Parse(attenTime + Attenlist[i].StartTime.ToString());
                var EndTime = DateTime.Parse(attenTime + Attenlist[i].EndTime.ToString());

                foreach (var item in strIds)//所有的打卡记录
                {
                    string time = item.SignTime.ToString("yyyy-MM-dd");
                    if (time != attenTime.TrimEnd())
                    {
                        continue;
                    }

                    if (count == 0)//第一个时间段
                    {
                        if (item.SignTime <= StartTime)
                        {
                            ii++;
                        }
                    }
                    else if (count == num)//最后一个时间段
                    {
                        if (item.SignTime >= EndTime)
                        {
                            ii++;
                        }
                    }
                    else//中间时间段
                    {
                        if (item.SignTime <= StartTime && item.SignTime >= EndTime)
                        {
                            //判断是不是已经打卡过,没有+1
                            var su = userIdList.Contains(item.userId.ToString());
                            if (!su)
                            {
                                userIdList.Add(item.userId.ToString());
                                ii++;
                            }
                        }
                    }

                }
                count++;
            }
            return ii;

        }
        #endregion
    }
}
