using hc.epm.Common;
using hc.epm.ViewModel;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace hc.epm.UI.Common
{
    public static class HelperExt
    {
        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="content"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static MvcHtmlString CutString(this HtmlHelper helper, string content, int length = 0)
        {
            string result = content.CutByLength(length, "...");
            return new MvcHtmlString(result);
        }

        /// <summary>
        /// 过滤特殊字符
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string FilterHtml(this string input)
        {
            if (input != string.Empty && input != null)
            {
                //string ihtml = input.ToLower();
                string ihtml = Regex.Replace(input, "<script", "&lt;script", RegexOptions.IgnoreCase);
                ihtml = Regex.Replace(input, "script>", "script&gt;", RegexOptions.IgnoreCase);
                ihtml = Regex.Replace(input, "<%", "&lt;%", RegexOptions.IgnoreCase);
                ihtml = Regex.Replace(input, "%>", "%&gt;", RegexOptions.IgnoreCase);
                ihtml = Regex.Replace(input, "<\\$", "&lt;$", RegexOptions.IgnoreCase);
                ihtml = Regex.Replace(input, "\\$>", "$&gt;", RegexOptions.IgnoreCase);
                //ihtml = ihtml.Replace("<script", "&lt;script");
                //ihtml = ihtml.Replace("script>", "script&gt;");
                //ihtml = ihtml.Replace("<%", "&lt;%");
                //ihtml = ihtml.Replace("%>", "%&gt;");
                //ihtml = ihtml.Replace("<$", "&lt;$");
                //ihtml = ihtml.Replace("$>", "$&gt;");
                return ihtml;
            }
            else
            {
                return string.Empty;
            }
        }

        public static MvcHtmlString ButtonOperate(this HtmlHelper helper, object module, object right, string text = "")
        {
            StringBuilder str = new StringBuilder();
            //if (string.IsNullOrEmpty(text))
            //{
            //    text = right.ToEnumReqByText<>();
            //}
            str.Append(" <a href=\"javascript:; \" >" + text + "</a>");
            return new MvcHtmlString(str.ToString());
        }

        public static MvcHtmlString ButtonRowOperate(this HtmlHelper helper, object module, object right, string text = "")
        {
            StringBuilder str = new StringBuilder();
            //if (string.IsNullOrEmpty(text))
            //{
            //    text = right.ToEnumReqByText<>();
            //}
            str.Append("<a href=\"javascript:; \" class=\"editor editor-link\"><i class=\"editor\"></i></a>");
            return new MvcHtmlString(str.ToString());
        }

        /// <summary>
        /// 下拉框选项生成
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="textCol"></param>
        /// <param name="valueCol"></param>
        /// <param name="enableDefault"></param>
        /// <param name="selectedValue"></param>
        /// <returns></returns>
        public static SelectList ToSelectList<T>(this List<T> items, string textCol = "Name", string valueCol = "Id", bool enableDefault = true, string selectedValue = "")
        {
            if (items == null)
            {
                items = new List<T>();
            }
            SelectList list = new SelectList(items, valueCol, textCol, selectedValue);
            if (enableDefault)
            {
                var tempList = list.ToList();
                tempList.Insert(0, DefalultItem);
                list = new SelectList(tempList, "Value", "Text", selectedValue);
            }
            return list;
        }




        /// <summary>
        /// 下拉框选项生成，自定义第一个默认项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="textCol"></param>
        /// <param name="valueCol"></param>
        /// <param name="defaultItem"></param>
        /// <param name="selectedValue"></param>
        /// <returns></returns>
        public static SelectList ToSelectList<T>(this List<T> items, string textCol = "Name", string valueCol = "Id", SelectListItem defaultItem = null, string selectedValue = "")
        {
            if (items == null)
            {
                items = new List<T>();
            }
            SelectList list = new SelectList(items, valueCol, textCol, selectedValue);
            if (defaultItem != null)
            {
                var tempList = list.ToList();
                tempList.Insert(0, defaultItem);
                list = new SelectList(tempList, "Value", "Text", selectedValue);
            }
            return list;
        }
        /// <summary>
        /// 下拉框选项默认值
        /// </summary>
        public static SelectListItem DefalultItem
        {
            get
            {
                return new SelectListItem { Text = "请选择", Value = "" };
            }
        }
        /// <summary>
        ///  获取是否启用selectlist
        /// </summary>
        /// <param name="isDefault">是否生成请选择</param>
        /// <param name="selected">选中项的值</param>
        /// <returns></returns>
        public static SelectList GetEnableList(bool isDefault = true, string selected = "0")
        {
            var list = Enum<EnumState>.AsEnumerable().Where(i => i == EnumState.Enable || i == EnumState.Disable).ToDictionary(i => i.ToString(), j => j.GetText()).ToList().ToSelectList("Value", "Key", isDefault, selected);
            return list;
        }
        /// <summary>
        /// 获取是否确认selectlist
        /// </summary>
        /// <param name="isDefault">是否生成请选择</param>
        /// <param name="selected">选中项的值</param>
        /// <returns></returns>
        public static SelectList GetConfirmList(bool isDefault = true, string selected = "0")
        {
            var list = Enum<EnumState>.AsEnumerable().Where(i => i == EnumState.Confirmed || i == EnumState.NoConfim).ToDictionary(i => i.ToString(), j => j.GetText()).ToList().ToSelectList("Value", "Key", isDefault, selected);
            return list;
        }

        public static ResultView<T> ToResultView<T>(this Result<T> result, bool isFilterError = false)
        {
            ResultView<T> model = new ResultView<T>();
            model.Data = result.Data;
            model.Flag = result.Flag == EResultFlag.Success;
            model.Message = "";
            if (result.Exception != null)
            {
                model.Message = result.Exception.Decription;
            }
            model.Other = "";
            if (result.Exception != null && !string.IsNullOrEmpty(result.Exception.Decription) && isFilterError)//直接截取处理异常
            {
                HttpContext.Current.Response.Redirect("/Home/Error?msg=" + model.Message);
                HttpContext.Current.Response.End();
            }
            return model;
        }

        /// <summary>
        /// 将枚举转换成下拉列表
        /// </summary>
        /// <param name="type">枚举</param>
        /// <param name="defaultItem">是否添加默认项</param>
        /// <param name="selectValue">默认选中项</param>
        /// <param name="removeValues">排除的值</param>
        /// <returns></returns>
        public static SelectList AsSelectList(this Type type, bool defaultItem, string selectValue = "", List<string> removeValues = null)
        {
            if (!type.IsEnum)
            {
                throw new NotSupportedException(string.Format("{0}必须为枚举类型。", type));
            }
            Array array = Enum.GetValues(type);
            List<SelectListItem> list = new List<SelectListItem>();
            if (array.Length > 0)
            {
                list = (from a in array.Cast<object>()
                        select new SelectListItem
                        {
                            Value = a.ToString(),
                            Text = (a as Enum).GetText()
                        }).ToList();
            }
            if (removeValues != null && removeValues.Count > 0)
            {
                list = list.Where(p => !removeValues.Contains(p.Value)).ToList();
            }
            if (defaultItem)
            {
                list.Insert(0, DefalultItem);
            }
            SelectList selectList = new SelectList(list, "Value", "Text", selectValue);
            return selectList;
        }

        /// <summary>
        /// 将枚举转化成List集合
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<EnumList> AsEnumList(this Type type)
        {
            if (!type.IsEnum)
            {
                throw new NotSupportedException(string.Format("{0}必须为枚举类型。", type));
            }
            Array array = Enum.GetValues(type);
            List<EnumList> list = new List<EnumList>();
            if (array.Length > 0)
            {
                list = (from a in array.Cast<object>()
                        select new EnumList
                        {
                            id = a.ToString(),
                            text = (a as Enum).GetText()
                        }).ToList();
            }
            return list;
        }
    }
}
