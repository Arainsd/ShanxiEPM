using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace hc.Plat.Common.Extend
{
    /// <summary>
    /// 集合扩展方法类
    /// </summary>
    public static class ListExtensionMethod
    {


        /// <summary>
        /// 串联IList&lt;T&gt;成字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="separator">分隔符</param>
        /// <param name="wrap">是否在字符串两端添加分隔符</param>
        /// <param name="includeEmpty">是否包含空字符串</param>
        /// <returns></returns>
        public static string JoinToString<T>(this IEnumerable<T> list, string separator, bool wrap = false, bool includeEmpty = false)
        {
            if (list == null || list.Count() == 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            if (wrap)
            {
                foreach (T t in list)
                {
                    string s = string.Empty;
                    if (t != null)
                        s = t.ToString();
                    if (includeEmpty || !string.IsNullOrEmpty(s))
                        sb.Append(s + separator);
                }
                if (sb.Length > 0)
                    sb.Insert(0, separator);
            }
            else
            {
                foreach (T t in list)
                {
                    string s = string.Empty;
                    if (t != null)
                        s = t.ToString();

                    if (includeEmpty || !string.IsNullOrEmpty(s))
                        sb.Append(separator + s);
                }
                if (sb.Length > 0)
                    sb.Remove(0, separator.Length);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 转换string集合为int集合
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<int> ToInt32List(this IList<string> list)
        {
            List<int> list_int = new List<int>();

            if (list == null || list.Count() == 0)
                return list_int;

            foreach (string str in list)
            {
                int i;
                if (int.TryParse(str, out i))
                    list_int.Add(i);
            }

            return list_int;
        }
        /// <summary>
        /// 转换string集合为long集合
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<long> ToLongList(this IList<string> list)
        {
            List<long> list_long = new List<long>();

            if (list == null || list.Count() == 0)
                return list_long;

            foreach (string str in list)
            {
                long i;
                if (long.TryParse(str, out i))
                    list_long.Add(i);
            }

            return list_long;
        }

        /// <summary>
        /// 追加一个List，不创建新的List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="appendList"></param>
        public static void Append<T>(this IList<T> list, IList<T> appendList)
        {
            foreach (T t in appendList)
                list.Add(t);
        }

        /// <summary>
        /// 添加项，仅在IList中没有此项时
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value"></param>
        public static void AddWhenNotExisted<T>(this IList<T> list, T value)
        {
            if (!list.Contains(value))
                list.Add(value);
        }

        /// <summary>
        /// 复制集合，引用类型只是新建集合后把原集合中对象添加进新集合，并未复制对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IList<T> Clone<T>(this IList<T> list)
        {
            IList<T> newList = new List<T>();
            if (list != null)
            {
                foreach (T t in list)
                    newList.Add(t);
            }

            return newList;
        }

        public static string GetSendDateType(string type)
        {
            string result = "";
            switch (type)
            {
                case "内墙粉刷施工":
                    result = "NQFSSG";
                    break;
                case "办公室施工":
                    result = "BGSSG";
                    break;
                case "公共卫生间改造施工":
                    result = "WSJSG";
                    break;
                case "员工卫浴间装修":
                    result = "YWWYJ";
                    break;
                case "站房层数":
                    result = "CS";
                    break;
                case "总面积":
                    result = "ZMJ";
                    break;
                case "厨房装修":
                    result = "YWCF";
                    break;
                case "餐厅装修":
                    result = "YWCT";
                    break;
                case "宿舍装修":
                    result = "YWSS";
                    break;
                case "宿舍间数":
                    result = "SSJS";
                    break;
                case "配电间施工":
                    result = "PDJ";
                    break;
                case "发电间施工":
                    result = "YWFDJ";
                    break;
                case "挑檐施工":
                    result = "YWTY";
                    break;
                case "站房包装施工":
                    result = "WBZXS";
                    break;
                case "形象墙施工":
                    result = "YWXXQ";
                    break;
                case "电气线路更换施工":
                    result = "XLSFGH";
                    break;
                case "施工单位编号":
                    result = "SGDWBH";
                    break;
                case "施工单位名称":
                    result = "SGDWMC";
                    break;
                case "施工单位项目经理":
                    result = "XMJL";
                    break;
                case "施工单位编号1":
                    result = "SGDWBH1";
                    break;
                case "施工单位名称1":
                    result = "SGDWMC1";
                    break;
                case "项目经理1":
                    result = "XMJL1";
                    break;
                case "施工单位编号2":
                    result = "SGDWBH2";
                    break;
                case "施工单位名称2":
                    result = "SGDWMC2";
                    break;
                case "项目经理2":
                    result = "XMJL2";
                    break;
                case "SGDWBH3":
                    result = "施工单位编号3";
                    break;
                case "施工单位名称3":
                    result = "SGDWMC3";
                    break;
                case "项目经理3":
                    result = "XMJL3";
                    break;
                case "施工单位编号4":
                    result = "SGDWBH4";
                    break;
                case "施工单位名称4":
                    result = "SGDWMC4";
                    break;
                case "项目经理4":
                    result = "XMJL4";
                    break;
                case "便利店面积":
                    result = "BLDMJ";
                    break;
                case "仓库施工":
                    result = "YWCK";
                    break;
                case "仓库面积":
                    result = "CKMJ";
                    break;
                case "罐区类型":
                    result = "LX";
                    break;
                case "罐池壁":
                    result = "YWGC";
                    break;
                case "罐池规格（长*宽*高）":
                    result = "GQGG";
                    break;
                case "防渗措施":
                    result = "FSCS";
                    break;
                case "防渗罐品牌":
                    result = "FSGPP";
                    break;
                case "总罐容":
                    result = "ZGR";
                    break;
                case "罐数":
                    result = "GS";
                    break;
                case "具备油气回收罐数":
                    result = "HSGS";
                    break;
                case "操作井类型":
                    result = "CZJLX";
                    break;
                case "操作井品牌":
                    result = "CZJPP";
                    break;
                case "有无防浮井":
                    result = "YWFFJ";
                    break;
                case "防浮措施":
                    result = "FFCS";
                    break;
                case "有无环保沟":
                    result = "YWHBG";
                    break;
                case "环保沟长度":
                    result = "HBGCD";
                    break;
                case "电气线路更换否":
                    result = "XLSFGH";
                    break;
                case "加油岛位数":
                    result = "JYDWS";
                    break;
                case "油气回收岛位数":
                    result = "YQHSDWS";
                    break;
                case "电气线路施工":
                    result = "XLSFGH";
                    break;
                case "油气回收管线":
                    result = "YQHSGX";
                    break;
                case "罩棚灯数":
                    result = "ZPDS";
                    break;
                case "罩棚灯品牌":
                    result = "ZPDPP";
                    break;
                case "罩棚应急灯数":
                    result = "ZPYJDS";
                    break;
                case "是否含环保沟":
                    result = "SFHHBG";
                    break;
                case "地坪面积":
                    result = "DPMJ";
                    break;
                case "带筋地坪":
                    result = "DJDP";
                    break;
                case "沥青路面":
                    result = "LQLM";
                    break;
                case "素混凝土":
                    result = "SHNT";
                    break;
                case "绿化面积":
                    result = "LHMJ";
                    break;
                case "结构类型":
                    result = "JGLX";
                    break;
                case "罩棚面积":
                    result = "ZPMJ";
                    break;
                case "檐口包装":
                    result = "SFHYKBZ";
                    break;
                case "檐口包装高度":
                    result = "YKGD";
                    break;
                case "檐口包装类型":
                    result = "YKBZLX";
                    break;
                case "立柱类型":
                    result = "LZLX";
                    break;
                case "立柱包装":
                    result = "LZSFDBZ";
                    break;
                case "彩钢瓦更换":
                    result = "SFGHCGY";
                    break;
                case "网架除锈刷漆":
                    result = "SFHCXSQ";
                    break;
                case "天沟更换":
                    result = "SFGHTG";
                    break;
                case "罩棚高度":
                    result = "ZPGD";
                    break;
                case "电气线路更换":
                    result = "XLSFGH";
                    break;
                case "罩棚排水方式":
                    result = "ZPPSFS";
                    break;
                case "檐口灯箱类型":
                    result = "YKDXLX";
                    break;
                case "围墙长度":
                    result = "WQCD";
                    break;
                case "挡土墙（长*高）":
                    result = "DTQ";
                    break;
                case "变压器安装":
                    result = "BYQAZ";
                    break;
                case "打水井":
                    result = "SFDLSJ";
                    break;
                case "隔油池施工":
                    result = "YWGYC";
                    break;
                case "高杆灯施工":
                    result = "YWGGD";
                    break;
                case "立柱广告牌施工":
                    result = "YWDLZP";
                    break;
                case "立柱牌高度":
                    result = "DLZPGD";
                    break;
                case "进出口指示牌":
                    result = "YWJCKZSP";
                    break;
                case "定制化物品配置":
                    result = "DZHWPPZ";
                    break;
                case "定制化物品配置金额":
                    result = "DZHWPPZJE";
                    break;
                case "消防器材箱":
                    result = "XFQCX";
                    break;
                default:
                    break;
            }
            return result;
        }

        public static string GetByteLength(string size)
        {
            string value = Regex.Replace(size, "[A-Z]", "", RegexOptions.IgnoreCase);
            string key = size.Replace(value, "");
            string result = "";
            switch (key)
            {
                case "B":
                    result = value;
                    break;
                case "KB":
                    result = (Math.Floor(Convert.ToDouble(value) * 1024)).ToString();
                    break;
                case "M":
                    result = (Math.Floor(Convert.ToDouble(value) * 1024 * 1024)).ToString();
                    break;
                case "GB":
                    result = (Math.Floor(Convert.ToDouble(value) * 1024 * 1024 * 1024)).ToString();
                    break;
                default:
                    break;
            }
            return result;
        }

        public static string GetFileType(string name)
        {
            string key = name.Substring(name.LastIndexOf('.'));
            string result = "6";
            switch (key)
            {
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".gif":
                    result = "1";
                    break;
                case ".xlsx":
                    result = "2";
                    break;
                case ".pdf":
                    result = "3";
                    break;
                case ".docx":
                    result = "4";
                    break;
                case ".txt":
                    result = "5";
                    break;
                case ".xls":
                    result = "7";
                    break;
                case ".doc":
                    result = "8";
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}
