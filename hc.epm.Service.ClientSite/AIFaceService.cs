using Baidu.Aip.Face;
using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.Service.Base;
using hc.epm.ViewModel;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.Service.ClientSite
{
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        /// <summary>
        /// 人脸识别API参数初始化
        /// </summary>
        /// <returns></returns>
        private Face GetFacaClient()
        {
            string API_KEY = "hDFL0bNfdMtIj1B5oCEyfHMG";
            string SECRET_KEY = "QygQEX73YOYL23IQIE3GLdyV0PnA7z43";
            Face client = new Face(API_KEY, SECRET_KEY);

            client.Timeout = 60000;  // 修改超时时间
            return client;
        }

        /// <summary>
        /// 人脸注册
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="image"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public Result<int> AddAIUserFaceInfo(long userId, string image, string source)
        {
            Result<int> resultObj = new Result<int>();
            resultObj.Data = -1;
            BasicDataContext basedb = new BasicDataContext();
            BusinessDataContext busdb = new BusinessDataContext();
            try
            {
                var imageType = "BASE64";
                var groupId = "group1";
                string control = "LOW";
                image = image.Substring(image.IndexOf(",") + 1);      //将‘，’以前的多余字符串删除
                // 如果有可选参数
                var options = new Dictionary<string, object>{
                                    {"quality_control", control},
                                    {"liveness_control", control}
                                };
                Face client = GetFacaClient();
                JObject result = new JObject();

                //人脸管理
                EPM_AIUserFace userFaceAI = new EPM_AIUserFace();
                //人脸日志表
                EPM_FaceOperateLog faceLog = new EPM_FaceOperateLog();
                //查询用户详情
                var userResult = DataOperateBasic<Base_User>.Get(basedb).GetModel(userId);
                //查询该用户是否已经注册的人脸信息
                userFaceAI = DataOperateBusiness<EPM_AIUserFace>.Get(busdb).GetList(t => t.UserId == userId).FirstOrDefault();
                //已存在人脸更新现有数据
                //是否已经存在人脸
                bool isExitsUserFace = false;
                //是否添加人脸
                bool isAddFace = false;
                if (userFaceAI != null && userFaceAI.IsSuccess)
                {
                    faceLog.APIType = FaceOperate.Update.ToString();
                    isExitsUserFace = true;
                }
                else  //不存在添加新数据
                {
                    isAddFace = true;
                    faceLog.APIType = FaceOperate.Add.ToString();
                    if (userFaceAI == null)
                    {
                        userFaceAI = new EPM_AIUserFace();
                        isExitsUserFace = false;
                    }
                    else
                    {
                        isExitsUserFace = true;
                    }
                    userFaceAI.IsSuccess = true;
                    userFaceAI.UserId = userId;
                    userFaceAI.UserName = userResult.UserName;
                    userFaceAI.UserPhone = userResult.Phone;
                    userFaceAI.ImageBase64 = "data:image/jpeg;base64," + image;
                    userFaceAI.ImageType = imageType;
                    userFaceAI.GroupId = groupId;
                    userFaceAI.LivenessControl = control;
                    userFaceAI.QualityControl = control;
                    userFaceAI.Source = source;
                }
                //日志
                var requestJson = new
                {
                    imageType = imageType,
                    groupId = groupId,
                    control = control,
                    userId = userId,
                    options = new
                    {
                        quality_control = control,
                        liveness_control = control
                    }
                };
                faceLog.ModelId = userFaceAI.Id;
                faceLog.RequestJson = requestJson.ToString();
                faceLog = SetCurrentUser(faceLog);
                try
                {
                    if (isAddFace)
                    {
                        result = client.UserAdd(image, imageType, groupId, userId.ToString(), options);
                    }
                    else
                    {
                        result = client.UserUpdate(image, imageType, groupId, userId.ToString(), options);
                    }

                    userFaceAI.LogId = result["log_id"].ToString();

                    if (result["error_code"].ToString() == "0" && result["error_msg"].ToString() == "SUCCESS")
                    {
                        var result_list = Newtonsoft.Json.JsonConvert.DeserializeObject(result["result"].ToString()) as JObject;

                        var result_location = Newtonsoft.Json.JsonConvert.DeserializeObject(result_list["location"].ToString()) as JObject;

                        userFaceAI.FaceToken = result_list["face_token"].ToString();
                        userFaceAI.Left = Convert.ToDecimal(result_location["left"]);
                        userFaceAI.Top = Convert.ToDecimal(result_location["top"]);
                        userFaceAI.Width = Convert.ToDecimal(result_location["width"]);
                        userFaceAI.Height = Convert.ToDecimal(result_location["height"]);
                        userFaceAI.Rotation = result_location["rotation"].ToString();
                        userFaceAI.Location = result_list["location"].ToString();
                        userFaceAI.IsSuccess = true;
                        userFaceAI = SetCurrentUser(userFaceAI);

                        faceLog.IsSuccess = true;

                        resultObj.Data = 1;
                    }
                    else
                    {
                        faceLog.IsSuccess = false;

                        userFaceAI.IsSuccess = false;
                    }
                }
                catch (Exception)
                {
                    faceLog.IsSuccess = false;
                    userFaceAI.IsSuccess = false;
                }
                faceLog.ResponseJson = result.ToString();
                if (isExitsUserFace)
                {
                    DataOperateBusiness<EPM_AIUserFace>.Get(busdb).Update(userFaceAI);
                }
                else
                {
                    DataOperateBusiness<EPM_AIUserFace>.Get(busdb).Add(userFaceAI);
                }
                DataOperateBusiness<EPM_FaceOperateLog>.Get(busdb).Add(faceLog);
                resultObj.Flag = EResultFlag.Success;

            }
            catch (Exception ex)
            {
                resultObj.Data = -1;
                resultObj.Flag = EResultFlag.Failure;
                resultObj.Exception = new ExceptionEx(ex, "AddAIUserFaceInfo");
            }
            finally
            {
                if (basedb.Database.Connection.State != ConnectionState.Closed)
                {
                    basedb.Database.Connection.Close();
                    basedb.Database.Connection.Dispose();
                }
                if (busdb.Database.Connection.State != ConnectionState.Closed)
                {
                    busdb.Database.Connection.Close();
                    busdb.Database.Connection.Dispose();
                }
            }

            return resultObj;
        }

        /// <summary>
        /// 人脸搜索
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> SearchUserFace(SignFaceAI model)
        {
            BasicDataContext basedb = new BasicDataContext();
            BusinessDataContext busdb = new BusinessDataContext();
            Result<int> resultObj = new Result<int>();
            resultObj.Data = -1;
            try
            {
                //查询用户是否注册人脸信息
                var userFaceList = DataOperateBusiness<EPM_AIUserFace>.Get(busdb).GetList(t => t.UserId == model.UserId);
                if (!userFaceList.Any())
                {
                    throw new Exception("您还没有注册人脸识别信息！");
                }
                else {
                    var imageType = "BASE64";
                    var groupIdList = "group1";
                    string control = "LOW";
                    model.Image = model.Image.Substring(model.Image.IndexOf(",") + 1);      //将‘，’以前的多余字符串删除
                    var options = new Dictionary<string, object>{
                                    {"quality_control", control},
                                    {"liveness_control", control},
                                    {"user_id", model.UserId.ToString()}
                                };
                    //签到表
                    Epm_SignInformation modelSign = new Epm_SignInformation();
                    //人脸日志表
                    EPM_FaceOperateLog faceLog = new EPM_FaceOperateLog();
                    Base_User userModel = DataOperateBasic<Base_User>.Get(basedb).GetModel(model.UserId.Value);
                    List<Epm_ProjectCompany> proComList = DataOperateBusiness<Epm_ProjectCompany>.Get(busdb).GetList(t => t.ProjectId == model.ProjectId && (t.PMId == model.UserId || t.LinkManId == model.UserId || t.SafeManId == model.UserId)).ToList();
                    string type = "";
                    if (proComList.Any())
                    {
                        type = string.Join(",", proComList.Select(t => t.Type));
                    }

                    modelSign.userId = model.UserId;
                    modelSign.userName = userModel.UserName;
                    modelSign.projectId = model.ProjectId;
                    modelSign.projectName = model.ProjectName;
                    modelSign.Latitude = model.Latitude;
                    modelSign.Longitude = model.Longitude;
                    modelSign.gasstationName = model.OilStationName;
                    modelSign.jobInfo = userModel.Post ?? "";
                    modelSign.type = type;
                    modelSign.picStrength = model.Image;
                    modelSign.SignTime = DateTime.Now;

                    //日志
                    var requestJson = new
                    {
                        imageType = imageType,
                        groupId = groupIdList,
                        options = new
                        {
                            quality_control = control,
                            liveness_control = control,
                            user_id = model.UserId
                        }
                    };
                    faceLog.ModelId = modelSign.Id;
                    faceLog.APIType = FaceOperate.Search.ToString();
                    faceLog.RequestJson = requestJson.ToString();

                    JObject result = new JObject();
                    Face client = GetFacaClient();
                    try
                    {
                        // 带参数调用人脸识别
                        result = client.Search(model.Image, imageType, groupIdList, options);
                    }
                    catch (Exception)
                    {
                        modelSign.SignResult = SignRes.Other.ToString();
                    }
                    if (result["error_code"].ToString() == "0" && result["error_msg"].ToString() == "SUCCESS")
                    {
                        var result_list = Newtonsoft.Json.JsonConvert.DeserializeObject(result["result"].ToString()) as JObject;
                        var user_list = result_list["user_list"];
                        var Obj = JArray.Parse(user_list.ToString());
                        foreach (var item in Obj)
                        {
                            //80分以上可以判断为同一人，此分值对应万分之一误识率
                            var score = Convert.ToInt32(item["score"]);
                            if (score > 80)
                            {
                                modelSign.SignResult = SignRes.Success.ToString();
                                faceLog.IsSuccess = true;
                                resultObj.Data = 1;
                                resultObj.Flag = EResultFlag.Success;
                            }
                            else
                            {
                                modelSign.SignResult = SignRes.NoFace.ToString();
                                faceLog.IsSuccess = false;
                            }
                        }
                    }
                    else
                    {
                        modelSign.SignResult = SignRes.Fail.ToString();
                    }

                    faceLog.ResponseJson = result.ToString();
                    faceLog = SetCurrentUser(faceLog);

                    DataOperateBusiness<EPM_FaceOperateLog>.Get(busdb).Add(faceLog);
                    var rows = DataOperateBusiness<Epm_SignInformation>.Get(busdb).Add(modelSign);
                    if (rows > 0)
                    {
                        resultObj.Flag = EResultFlag.Success;
                    }
                }
            }
            catch (Exception ex)
            {
                resultObj.Data = -1;
                resultObj.Flag = EResultFlag.Failure;
                resultObj.Exception = new ExceptionEx(ex, "SearchUserFace");
            }
            finally
            {
                if (basedb.Database.Connection.State != ConnectionState.Closed)
                {
                    basedb.Database.Connection.Close();
                    basedb.Database.Connection.Dispose();
                }
                if (busdb.Database.Connection.State != ConnectionState.Closed)
                {
                    busdb.Database.Connection.Close();
                    busdb.Database.Connection.Dispose();
                }
            }
            return resultObj;
        }

        /// <summary>
        /// 根据用户ID获取该用户注册的人脸信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Result<EPM_AIUserFace> GetAIUserFace(long userId)
        {
            Result<EPM_AIUserFace> result = new Result<EPM_AIUserFace>();
            try
            {
                var version = DataOperateBusiness<EPM_AIUserFace>.Get().Single(p => p.UserId == userId && p.IsSuccess);
                result.Data = version;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetAIUserFace");
            }
            return result;
        }

        #region 考勤信息
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_SignInformation>> GetSignInformationList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_SignInformation>> result = new Result<List<Epm_SignInformation>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_SignInformation>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetSignInformationList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_SignInformation> GetSignInformationModel(long id)
        {
            Result<Epm_SignInformation> result = new Result<Epm_SignInformation>();
            try
            {
                var model = DataOperateBusiness<Epm_SignInformation>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetSignInformationModel");
            }
            return result;
        }
        #endregion
    }
}
