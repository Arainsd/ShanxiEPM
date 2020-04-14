using System.Collections.Generic;
using System.Data;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.Util;
using System.ComponentModel;
using hc.epm.Web.ClientProxy;
using hc.epm.ViewModel;
using System.Configuration;

/// <summary>
/// 基于NPOI 2.4 版本封装的Excel操作类
/// </summary>
public class ExcelHelperNew
{
    /// <summary>
    /// Excel导入成DataTable
    /// </summary>
    /// <param name="file">导入路径(包含文件名与扩展名)</param>
    /// <returns></returns>
    public static DataTable ExcelToTable(string file)
    {
        DataTable dt = new DataTable();
        IWorkbook workbook;
        string fileExt = Path.GetExtension(file).ToLower();
        using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
        {
            //XSSFWorkbook 适用XLSX格式，HSSFWorkbook 适用XLS格式
            if (fileExt == ".xlsx") { workbook = new XSSFWorkbook(fs); } else if (fileExt == ".xls") { workbook = new HSSFWorkbook(fs); } else { workbook = null; }
            if (workbook == null) { return null; }
            ISheet sheet = workbook.GetSheetAt(0);

            //表头  
            IRow header = sheet.GetRow(sheet.FirstRowNum);
            List<int> columns = new List<int>();
            for (int i = 0; i < header.LastCellNum; i++)
            {
                object obj = GetValueType(header.GetCell(i));
                if (obj == null || obj.ToString() == string.Empty)
                {
                    dt.Columns.Add(new DataColumn("Columns" + i.ToString()));
                }
                else
                    dt.Columns.Add(new DataColumn(obj.ToString()));
                columns.Add(i);
            }
            //数据  
            for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
            {
                DataRow dr = dt.NewRow();
                bool hasValue = false;
                foreach (int j in columns)
                {
                    dr[j] = GetValueType(sheet.GetRow(i).GetCell(j));
                    if (dr[j] != null && dr[j].ToString() != string.Empty)
                    {
                        hasValue = true;
                    }
                }
                if (hasValue)
                {
                    dt.Rows.Add(dr);
                }
            }
        }
        return dt;
    }

    /// <summary>
    /// Datable导出成Excel
    /// </summary>
    /// <param name="dt">生成的DataTable数据集</param>
    /// <param name="file">导出路径(包括文件名与扩展名)</param>
    /// <param name="recordErrorList">记录失败集合（集合为空，导出全部，否则只导出记录集合）</param>
    public static void TableToExcel(DataTable dt, string file, List<int> recordErrorList)
    {
        IWorkbook workbook;
        string fileExt = Path.GetExtension(file).ToLower();
        if (fileExt == ".xlsx")
        {
            workbook = new XSSFWorkbook();
        }
        else if (fileExt == ".xls")
        {
            workbook = new HSSFWorkbook();
        }
        else
        {
            workbook = null;
        }
        if (workbook == null)
        {
            return;
        }
        ISheet sheet = string.IsNullOrEmpty(dt.TableName) ? workbook.CreateSheet("Sheet1") : workbook.CreateSheet(dt.TableName);
        //表头  
        IRow row = sheet.CreateRow(0);
        for (int i = 0; i < dt.Columns.Count; i++)
        {
            ICell cell = row.CreateCell(i);
            cell.SetCellValue(dt.Columns[i].ColumnName);
        }
        if (recordErrorList.Count == 0)
        {
            //数据全部导出  
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IRow row1 = sheet.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ICell cell = row1.CreateCell(j);
                    cell.SetCellValue(dt.Rows[i][j].ToString());
                }
            }
        }
        else
        {
            //筛选出错误数据，进行导出  
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < recordErrorList.Count; j++)
                {
                    int record = recordErrorList[j];
                    if (i == record)
                    {
                        IRow row1 = sheet.CreateRow(j + 1);
                        for (int k = 0; k < dt.Columns.Count; k++)
                        {
                            ICell cell = row1.CreateCell(k);
                            cell.SetCellValue(dt.Rows[i][k].ToString());
                        }
                    }
                }

            }
        }


        //转为字节数组  
        MemoryStream stream = new MemoryStream();
        workbook.Write(stream);
        var buf = stream.ToArray();

        //保存为Excel文件  
        using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
        {
            fs.Write(buf, 0, buf.Length);
            fs.Flush();
        }
    }

    /// <summary>
    /// 获取单元格类型
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    private static object GetValueType(ICell cell)
    {
        if (cell == null)
            return null;
        switch (cell.CellType)
        {
            case CellType.Blank: //BLANK:  
                return null;
            case CellType.Boolean: //BOOLEAN:  
                return cell.BooleanCellValue;
            case CellType.Numeric: //NUMERIC:  
                return cell.NumericCellValue;
            case CellType.String: //STRING:  
                return cell.StringCellValue;
            case CellType.Error: //ERROR:  
                return cell.ErrorCellValue;
            case CellType.Formula: //FORMULA:  
            default:
                return "=" + cell.CellFormula;
        }
    }

    public static bool ExportForExecl(WeeklyView weeklyView)
    {
        var pathUrl = ConfigurationManager.AppSettings["ImportOrExportPath"];
        FileStream file = new FileStream(pathUrl + "周报模板.xls", FileMode.Open, FileAccess.Read);
        HSSFWorkbook hssfworkbook = new HSSFWorkbook(file);
        ISheet sheet1 = hssfworkbook.GetSheet("项目汇总表");
        var rowNo = 4;
        for (int i = 0; i < weeklyView.projectCounts.Count; i++)
        {
            sheet1.GetRow(i + rowNo).GetCell(0).SetCellValue(weeklyView.projectCounts[i].CompanyName);
            sheet1.GetRow(i + rowNo).GetCell(1).SetCellValue(weeklyView.projectCounts[i].Count.ToString());
            sheet1.GetRow(i + rowNo).GetCell(2).SetCellValue(weeklyView.projectCounts[i].NoStartCount.ToString());
            sheet1.GetRow(i + rowNo).GetCell(3).SetCellValue(weeklyView.projectCounts[i].DesignSchemeCount.ToString());
            sheet1.GetRow(i + rowNo).GetCell(4).SetCellValue(weeklyView.projectCounts[i].TenderingApplyCount.ToString());
            sheet1.GetRow(i + rowNo).GetCell(5).SetCellValue(weeklyView.projectCounts[i].StartCount.ToString());
            sheet1.GetRow(i + rowNo).GetCell(6).SetCellValue(weeklyView.projectCounts[i].FinshCount.ToString());
            sheet1.GetRow(i + rowNo).GetCell(7).SetCellValue(weeklyView.projectCounts[i].ProjectPolitCount.ToString());
            sheet1.GetRow(i + rowNo).GetCell(8).SetCellValue(weeklyView.projectCounts[i].CompletionAcceptanceCount.ToString());
            sheet1.GetRow(i + rowNo).GetCell(9).SetCellValue(weeklyView.projectCounts[i].CapitalTransferCount.ToString());
            sheet1.GetRow(i + rowNo).GetCell(10).SetCellValue("");
            sheet1.GetRow(i + rowNo).GetCell(11).SetCellValue(weeklyView.projectCounts[i].AcceptanceCount.ToString());
            sheet1.GetRow(i + rowNo).GetCell(12).SetCellValue(weeklyView.projectCounts[i].ConstructionCount.ToString());
        }
        ISheet sheet2 = hssfworkbook.GetSheet("项目汇总");
        var rowNo1 = 2;
        for (int ii = 0; ii < weeklyView.projectViews.Count; ii++)
        {
            sheet2.GetRow(ii + rowNo1).GetCell(0).SetCellValue(weeklyView.projectViews[ii].CompanyName);
            sheet2.GetRow(ii + rowNo1).GetCell(1).SetCellValue(ii + 1);
            sheet2.GetRow(ii + rowNo1).GetCell(2).SetCellValue(weeklyView.projectViews[ii].Name);
            sheet2.GetRow(ii + rowNo1).GetCell(3).SetCellValue(weeklyView.projectViews[ii].ApprovalNo);
            sheet2.GetRow(ii + rowNo1).GetCell(4).SetCellValue(weeklyView.projectViews[ii].ProjectNatureName);
            sheet2.GetRow(ii + rowNo1).GetCell(5).SetCellValue(weeklyView.projectViews[ii].Amount.ToString());
            sheet2.GetRow(ii + rowNo1).GetCell(6).SetCellValue(weeklyView.projectViews[ii].GasDailySales.ToString());
            sheet2.GetRow(ii + rowNo1).GetCell(7).SetCellValue(weeklyView.projectViews[ii].ReplyDate.ToString());
            sheet2.GetRow(ii + rowNo1).GetCell(8).SetCellValue(weeklyView.projectViews[ii].ReplyTime.ToString());
            sheet2.GetRow(ii + rowNo1).GetCell(9).SetCellValue(weeklyView.projectViews[ii].DesignSchemeTime.ToString());
            sheet2.GetRow(ii + rowNo1).GetCell(10).SetCellValue(weeklyView.projectViews[ii].BidResultTime.ToString());
            sheet2.GetRow(ii + rowNo1).GetCell(11).SetCellValue(weeklyView.projectViews[ii].PlanWorkStartTime.ToString());
            sheet2.GetRow(ii + rowNo1).GetCell(12).SetCellValue(weeklyView.projectViews[ii].PlanWorkEndTime.ToString());
            sheet2.GetRow(ii + rowNo1).GetCell(13).SetCellValue(weeklyView.projectViews[ii].ProjectPolitTime.ToString());
            sheet2.GetRow(ii + rowNo1).GetCell(14).SetCellValue(weeklyView.projectViews[ii].Limit.ToString());
            sheet2.GetRow(ii + rowNo1).GetCell(15).SetCellValue(weeklyView.projectViews[ii].ReplyTime.ToString());
            sheet2.GetRow(ii + rowNo1).GetCell(16).SetCellValue(weeklyView.projectViews[ii].RecTime.ToString());
            sheet2.GetRow(ii + rowNo1).GetCell(17).SetCellValue(weeklyView.projectViews[ii].ReplyTime.ToString());
            sheet2.GetRow(ii + rowNo1).GetCell(18).SetCellValue(weeklyView.projectViews[ii].DesignUnit);
            sheet2.GetRow(ii + rowNo1).GetCell(19).SetCellValue(weeklyView.projectViews[ii].WorkUnit);
            sheet2.GetRow(ii + rowNo1).GetCell(20).SetCellValue(weeklyView.projectViews[ii].WorkUnitPMName);
            sheet2.GetRow(ii + rowNo1).GetCell(21).SetCellValue(weeklyView.projectViews[ii].SupervisorUnit);
            sheet2.GetRow(ii + rowNo1).GetCell(22).SetCellValue(weeklyView.projectViews[ii].SupervisorUnitName);
            //sheet2.GetRow(ii + rowNo1).GetCell(23).SetCellValue(weeklyView.projectViews[ii].ProjectState);
        }
        ISheet sheet3 = hssfworkbook.GetSheet("在建");
        var rowNo2 = 3;
        for (int i3 = 0; i3 < weeklyView.projectViews14.Count; i3++)
        {
            sheet3.GetRow(i3 + rowNo2).GetCell(0).SetCellValue(i3 + 1);
            sheet3.GetRow(i3 + rowNo2).GetCell(1).SetCellValue(weeklyView.projectViews14[i3].Name);
            sheet3.GetRow(i3 + rowNo2).GetCell(2).SetCellValue(weeklyView.projectViews14[i3].ProjectNatureName);
            sheet3.GetRow(i3 + rowNo2).GetCell(3).SetCellValue(weeklyView.projectViews14[i3].PlanWorkStartTime.ToString());
            sheet3.GetRow(i3 + rowNo2).GetCell(4).SetCellValue(weeklyView.projectViews14[i3].PlanWorkEndTime.ToString());
            sheet3.GetRow(i3 + rowNo2).GetCell(5).SetCellValue(weeklyView.projectViews14[i3].WorkUnit.ToString());
            sheet3.GetRow(i3 + rowNo2).GetCell(6).SetCellValue(weeklyView.projectViews14[i3].WorkUnitPMName);
            sheet3.GetRow(i3 + rowNo2).GetCell(7).SetCellValue(weeklyView.projectViews14[i3].SupervisorUnit);
            sheet3.GetRow(i3 + rowNo2).GetCell(8).SetCellValue(weeklyView.projectViews14[i3].SupervisorUnitName);
            sheet3.GetRow(i3 + rowNo2).GetCell(9).SetCellValue(weeklyView.projectViews14[i3].ProjectState.ToString());
        }
        ISheet sheet4 = hssfworkbook.GetSheet("新增汇总表");
        var rowNo4 = 4;
        for (int i4 = 0; i4 < weeklyView.projectCounts.Count; i4++)
        {
            sheet3.GetRow(i4 + rowNo4).GetCell(0).SetCellValue(weeklyView.projectCounts[i4].CompanyName);
            sheet3.GetRow(i4 + rowNo4).GetCell(1).SetCellValue(weeklyView.projectCounts[i4].Count.ToString());
            sheet3.GetRow(i4 + rowNo4).GetCell(2).SetCellValue(weeklyView.projectCounts[i4].Count.ToString());
            sheet3.GetRow(i4 + rowNo4).GetCell(3).SetCellValue(weeklyView.projectCounts[i4].NoStartCount.ToString());
            sheet3.GetRow(i4 + rowNo4).GetCell(4).SetCellValue(weeklyView.projectCounts[i4].DesignSchemeCount.ToString());
            sheet3.GetRow(i4 + rowNo4).GetCell(5).SetCellValue(weeklyView.projectCounts[i4].TenderingApplyCount.ToString());
            sheet3.GetRow(i4 + rowNo4).GetCell(6).SetCellValue(weeklyView.projectCounts[i4].StartCount.ToString());
            sheet3.GetRow(i4 + rowNo4).GetCell(7).SetCellValue(weeklyView.projectCounts[i4].FinshCount.ToString());
            sheet3.GetRow(i4 + rowNo4).GetCell(8).SetCellValue(weeklyView.projectCounts[i4].Count.ToString());
            sheet3.GetRow(i4 + rowNo4).GetCell(9).SetCellValue(weeklyView.projectCounts[i4].CompletionAcceptanceCount.ToString());
            sheet3.GetRow(i4 + rowNo4).GetCell(10).SetCellValue(weeklyView.projectCounts[i4].CapitalTransferCount.ToString());
            sheet3.GetRow(i4 + rowNo4).GetCell(11).SetCellValue("");
            sheet3.GetRow(i4 + rowNo4).GetCell(12).SetCellValue(weeklyView.projectCounts[i4].ConstructionCount.ToString());
        }
        ISheet sheet5 = hssfworkbook.GetSheet("新增汇总");
        var rowNo5 = 4;
        for (int i5 = 0; i5 < weeklyView.projectViews7.Count; i5++)
        {
            sheet5.GetRow(i5 + rowNo5).GetCell(0).SetCellValue(weeklyView.projectViews7[i5].CompanyName);
            sheet5.GetRow(i5 + rowNo5).GetCell(1).SetCellValue(i5 + 1);
            sheet5.GetRow(i5 + rowNo5).GetCell(2).SetCellValue(weeklyView.projectViews7[i5].Name);
            sheet5.GetRow(i5 + rowNo5).GetCell(3).SetCellValue(weeklyView.projectViews7[i5].ApprovalNo);
            sheet5.GetRow(i5 + rowNo5).GetCell(4).SetCellValue(weeklyView.projectViews7[i5].ProjectNatureName);
            sheet5.GetRow(i5 + rowNo5).GetCell(5).SetCellValue(weeklyView.projectViews7[i5].Amount.ToString());
            sheet5.GetRow(i5 + rowNo5).GetCell(6).SetCellValue(weeklyView.projectViews7[i5].GasDailySales.ToString());
            sheet5.GetRow(i5 + rowNo5).GetCell(7).SetCellValue(weeklyView.projectViews7[i5].ReplyDate.ToString());
            sheet5.GetRow(i5 + rowNo5).GetCell(8).SetCellValue(weeklyView.projectViews7[i5].ReplyTime.ToString());
            sheet5.GetRow(i5 + rowNo5).GetCell(9).SetCellValue(weeklyView.projectViews7[i5].DesignSchemeTime.ToString());
            sheet5.GetRow(i5 + rowNo5).GetCell(10).SetCellValue(weeklyView.projectViews7[i5].BidResultTime.ToString());
            sheet5.GetRow(i5 + rowNo5).GetCell(11).SetCellValue(weeklyView.projectViews7[i5].PlanWorkStartTime.ToString());
            sheet5.GetRow(i5 + rowNo5).GetCell(12).SetCellValue(weeklyView.projectViews7[i5].PlanWorkEndTime.ToString());
            sheet5.GetRow(i5 + rowNo5).GetCell(13).SetCellValue(weeklyView.projectViews7[i5].ProjectPolitTime.ToString());
            sheet5.GetRow(i5 + rowNo5).GetCell(14).SetCellValue(weeklyView.projectViews7[i5].Limit.ToString());
            sheet5.GetRow(i5 + rowNo5).GetCell(15).SetCellValue(weeklyView.projectViews7[i5].FinanceTime.ToString());
            sheet5.GetRow(i5 + rowNo5).GetCell(16).SetCellValue(weeklyView.projectViews7[i5].RecTime.ToString());
            sheet5.GetRow(i5 + rowNo5).GetCell(17).SetCellValue(weeklyView.projectViews7[i5].FinanceTime.ToString());
            sheet5.GetRow(i5 + rowNo5).GetCell(18).SetCellValue(weeklyView.projectViews7[i5].DesignUnit);
            sheet5.GetRow(i5 + rowNo5).GetCell(19).SetCellValue(weeklyView.projectViews7[i5].WorkUnit);
            sheet5.GetRow(i5 + rowNo5).GetCell(20).SetCellValue(weeklyView.projectViews7[i5].WorkUnitPMName);
            sheet5.GetRow(i5 + rowNo5).GetCell(21).SetCellValue(weeklyView.projectViews7[i5].SupervisorUnit);
            sheet5.GetRow(i5 + rowNo5).GetCell(22).SetCellValue(weeklyView.projectViews7[i5].SupervisorUnitName);
            sheet5.GetRow(i5 + rowNo5).GetCell(23).SetCellValue(weeklyView.projectViews7[i5].ProjectState);
        }
        ISheet sheet6 = hssfworkbook.GetSheet("未完成设计");
        var rowNo6 = 3;
        for (int i6 = 0; i6 < weeklyView.projectViews2.Count; i6++)
        {
            sheet6.GetRow(i6 + rowNo6).GetCell(0).SetCellValue(weeklyView.projectViews2[i6].CompanyName);
            sheet6.GetRow(i6 + rowNo6).GetCell(1).SetCellValue(i6 + 1);
            sheet6.GetRow(i6 + rowNo6).GetCell(2).SetCellValue(weeklyView.projectViews2[i6].Name);
            sheet6.GetRow(i6 + rowNo6).GetCell(3).SetCellValue(weeklyView.projectViews2[i6].ProjectNatureName);
            sheet6.GetRow(i6 + rowNo6).GetCell(4).SetCellValue(weeklyView.projectViews2[i6].ReplyDate.ToString());
            sheet6.GetRow(i6 + rowNo6).GetCell(5).SetCellValue(weeklyView.projectViews2[i6].DesignSchemeTime.ToString());
            sheet6.GetRow(i6 + rowNo6).GetCell(6).SetCellValue(weeklyView.projectViews2[i6].ProjectState.ToString());
        }
        ISheet sheet7 = hssfworkbook.GetSheet("未完成招标");
        var rowNo7 = 3;
        for (int i7 = 0; i7 < weeklyView.projectViews5.Count; i7++)
        {
            sheet7.GetRow(i7 + rowNo7).GetCell(0).SetCellValue(weeklyView.projectViews5[i7].CompanyName);
            sheet7.GetRow(i7 + rowNo7).GetCell(1).SetCellValue(i7 + 1);
            sheet7.GetRow(i7 + rowNo7).GetCell(2).SetCellValue(weeklyView.projectViews5[i7].Name);
            sheet7.GetRow(i7 + rowNo7).GetCell(3).SetCellValue(weeklyView.projectViews5[i7].ProjectNatureName);
            sheet7.GetRow(i7 + rowNo7).GetCell(4).SetCellValue(weeklyView.projectViews5[i7].DesignSchemeTime.ToString());
            sheet7.GetRow(i7 + rowNo7).GetCell(5).SetCellValue(weeklyView.projectViews5[i7].BidResultTime.ToString());
            sheet7.GetRow(i7 + rowNo7).GetCell(6).SetCellValue(weeklyView.projectViews5[i7].ProjectState.ToString());
        }
        ISheet sheet8 = hssfworkbook.GetSheet("完成未投运");
        var rowNo8 = 3;
        for (int i8 = 0; i8 < weeklyView.projectViews8.Count; i8++)
        {
            sheet8.GetRow(i8 + rowNo8).GetCell(0).SetCellValue(weeklyView.projectViews8[i8].CompanyName);
            sheet8.GetRow(i8 + rowNo8).GetCell(1).SetCellValue(i8 + 1);
            sheet8.GetRow(i8 + rowNo8).GetCell(2).SetCellValue(weeklyView.projectViews8[i8].Name);
            sheet8.GetRow(i8 + rowNo8).GetCell(3).SetCellValue(weeklyView.projectViews8[i8].ProjectNatureName);
            sheet8.GetRow(i8 + rowNo8).GetCell(4).SetCellValue(weeklyView.projectViews8[i8].PlanWorkEndTime.ToString());
            sheet8.GetRow(i8 + rowNo8).GetCell(5).SetCellValue(weeklyView.projectViews8[i8].ProjectPolitTime.ToString());
            sheet8.GetRow(i8 + rowNo8).GetCell(6).SetCellValue(weeklyView.projectViews8[i8].ProjectState.ToString());
        }
        ISheet sheet9 = hssfworkbook.GetSheet("正在施工");
        var rowNo9 = 3;
        for (int i9 = 0; i9 < weeklyView.projectViews4.Count; i9++)
        {
            sheet9.GetRow(i9 + rowNo9).GetCell(0).SetCellValue(weeklyView.projectViews4[i9].CompanyName);
            sheet9.GetRow(i9 + rowNo9).GetCell(1).SetCellValue(i9 + 1);
            sheet9.GetRow(i9 + rowNo9).GetCell(2).SetCellValue(weeklyView.projectViews4[i9].Name);
            sheet9.GetRow(i9 + rowNo9).GetCell(3).SetCellValue(weeklyView.projectViews4[i9].ProjectNatureName);
            sheet9.GetRow(i9 + rowNo9).GetCell(4).SetCellValue(weeklyView.projectViews4[i9].PlanWorkStartTime.ToString());
            sheet9.GetRow(i9 + rowNo9).GetCell(5).SetCellValue(weeklyView.projectViews4[i9].Limit.ToString());
            sheet9.GetRow(i9 + rowNo9).GetCell(6).SetCellValue(weeklyView.projectViews4[i9].ConsumptionPeriod.ToString());
            sheet9.GetRow(i9 + rowNo9).GetCell(7).SetCellValue(weeklyView.projectViews4[i9].SurplusLimit.ToString());
            sheet9.GetRow(i9 + rowNo9).GetCell(8).SetCellValue(weeklyView.projectViews4[i9].WorkSchedule.ToString());
            sheet9.GetRow(i9 + rowNo9).GetCell(9).SetCellValue(weeklyView.projectViews4[i9].WorkUnit);
            sheet9.GetRow(i9 + rowNo9).GetCell(10).SetCellValue(weeklyView.projectViews4[i9].WorkUnitPMName);
            sheet9.GetRow(i9 + rowNo9).GetCell(11).SetCellValue(weeklyView.projectViews4[i9].SupervisorUnit);
            sheet9.GetRow(i9 + rowNo9).GetCell(12).SetCellValue(weeklyView.projectViews4[i9].SupervisorUnitName);
            sheet9.GetRow(i9 + rowNo9).GetCell(13).SetCellValue(weeklyView.projectViews4[i9].ProjectState);
        }
        ISheet sheet10 = hssfworkbook.GetSheet("改造汇总表");
        var rowNo10 = 3;
        for (int i10 = 0; i10 < weeklyView.projectCounts.Count; i10++)
        {
            sheet10.GetRow(i10 + rowNo10).GetCell(0).SetCellValue(weeklyView.projectCounts[i10].CompanyName);
            sheet10.GetRow(i10 + rowNo10).GetCell(1).SetCellValue(weeklyView.projectCounts[i10].Count.ToString());
            sheet10.GetRow(i10 + rowNo10).GetCell(2).SetCellValue(weeklyView.projectCounts[i10].NoStartCount.ToString());
            sheet10.GetRow(i10 + rowNo10).GetCell(3).SetCellValue(weeklyView.projectCounts[i10].DesignSchemeCount.ToString());
            sheet10.GetRow(i10 + rowNo10).GetCell(4).SetCellValue(weeklyView.projectCounts[i10].TenderingApplyCount.ToString());
            sheet10.GetRow(i10 + rowNo10).GetCell(5).SetCellValue(weeklyView.projectCounts[i10].StartCount.ToString());
            sheet10.GetRow(i10 + rowNo10).GetCell(6).SetCellValue(weeklyView.projectCounts[i10].FinshCount.ToString());
            sheet10.GetRow(i10 + rowNo10).GetCell(7).SetCellValue(weeklyView.projectCounts[i10].ProjectPolitCount.ToString());
            sheet10.GetRow(i10 + rowNo10).GetCell(8).SetCellValue(weeklyView.projectCounts[i10].CompletionAcceptanceCount.ToString());
            sheet10.GetRow(i10 + rowNo10).GetCell(9).SetCellValue(weeklyView.projectCounts[i10].CapitalTransferCount.ToString());
            sheet10.GetRow(i10 + rowNo10).GetCell(10).SetCellValue(weeklyView.projectCounts[i10].ConstructionCount.ToString());
            sheet10.GetRow(i10 + rowNo10).GetCell(11).SetCellValue("");
            sheet10.GetRow(i10 + rowNo10).GetCell(12).SetCellValue(weeklyView.projectCounts[i10].BeingBuiltCount.ToString());
        }
        ISheet sheet11 = hssfworkbook.GetSheet("改造汇总");
        var rowNo11 = 2;
        for (int i11 = 0; i11 < weeklyView.projectViews11.Count; i11++)
        {
            sheet11.GetRow(i11 + rowNo11).GetCell(0).SetCellValue(weeklyView.projectViews11[i11].CompanyName);
            sheet11.GetRow(i11 + rowNo11).GetCell(1).SetCellValue(i11 + 1);
            sheet11.GetRow(i11 + rowNo11).GetCell(2).SetCellValue(weeklyView.projectViews11[i11].Name);
            sheet11.GetRow(i11 + rowNo11).GetCell(3).SetCellValue(weeklyView.projectViews11[i11].ApprovalNo);
            sheet11.GetRow(i11 + rowNo11).GetCell(4).SetCellValue(weeklyView.projectViews11[i11].ProjectNatureName);
            sheet11.GetRow(i11 + rowNo11).GetCell(5).SetCellValue(weeklyView.projectViews11[i11].Amount.ToString());
            sheet11.GetRow(i11 + rowNo11).GetCell(6).SetCellValue(weeklyView.projectViews11[i11].GasDailySales.ToString());
            sheet11.GetRow(i11 + rowNo11).GetCell(7).SetCellValue(weeklyView.projectViews11[i11].ReplyDate.ToString());
            sheet11.GetRow(i11 + rowNo11).GetCell(8).SetCellValue(weeklyView.projectViews11[i11].DesignSchemeTime.ToString());
            sheet11.GetRow(i11 + rowNo11).GetCell(9).SetCellValue(weeklyView.projectViews11[i11].BidResultTime.ToString());
            sheet11.GetRow(i11 + rowNo11).GetCell(10).SetCellValue(weeklyView.projectViews11[i11].PlanWorkStartTime.ToString());
            sheet11.GetRow(i11 + rowNo11).GetCell(11).SetCellValue(weeklyView.projectViews11[i11].PlanWorkEndTime.ToString());
            sheet11.GetRow(i11 + rowNo11).GetCell(12).SetCellValue(weeklyView.projectViews11[i11].ProjectPolitTime.ToString());
            sheet11.GetRow(i11 + rowNo11).GetCell(13).SetCellValue(weeklyView.projectViews11[i11].Limit.ToString());
            sheet11.GetRow(i11 + rowNo11).GetCell(14).SetCellValue(weeklyView.projectViews11[i11].ReplyTime.ToString());
            sheet11.GetRow(i11 + rowNo11).GetCell(15).SetCellValue(weeklyView.projectViews11[i11].RecTime.ToString());
            sheet11.GetRow(i11 + rowNo11).GetCell(16).SetCellValue(weeklyView.projectViews11[i11].ReplyTime.ToString());
            sheet11.GetRow(i11 + rowNo11).GetCell(17).SetCellValue(weeklyView.projectViews11[i11].DesignUnit);
            sheet11.GetRow(i11 + rowNo11).GetCell(18).SetCellValue(weeklyView.projectViews11[i11].WorkUnit);
            sheet11.GetRow(i11 + rowNo11).GetCell(19).SetCellValue(weeklyView.projectViews11[i11].WorkUnitPMName);
            sheet11.GetRow(i11 + rowNo11).GetCell(20).SetCellValue(weeklyView.projectViews11[i11].SupervisorUnit);
            sheet11.GetRow(i11 + rowNo11).GetCell(21).SetCellValue(weeklyView.projectViews11[i11].SupervisorUnitName);
            sheet11.GetRow(i11 + rowNo11).GetCell(22).SetCellValue(weeklyView.projectViews11[i11].ProjectState);
        }
        ISheet sheet12 = hssfworkbook.GetSheet("未开工项目");
        var rowNo12 = 3;
        for (int i12 = 0; i12 < weeklyView.projectViews12.Count; i12++)
        {
            sheet12.GetRow(i12 + rowNo12).GetCell(0).SetCellValue(weeklyView.projectViews12[i12].CompanyName);
            sheet12.GetRow(i12 + rowNo12).GetCell(1).SetCellValue(i12 + 1);
            sheet12.GetRow(i12 + rowNo12).GetCell(2).SetCellValue(weeklyView.projectViews12[i12].Name);
            sheet12.GetRow(i12 + rowNo12).GetCell(3).SetCellValue(weeklyView.projectViews12[i12].ProjectNatureName);
            sheet12.GetRow(i12 + rowNo12).GetCell(4).SetCellValue(weeklyView.projectViews12[i12].ReplyDate.ToString());
            sheet12.GetRow(i12 + rowNo12).GetCell(5).SetCellValue(weeklyView.projectViews12[i12].DesignSchemeTime.ToString());
            sheet12.GetRow(i12 + rowNo12).GetCell(6).SetCellValue(weeklyView.projectViews12[i12].DesignUnit);
            sheet12.GetRow(i12 + rowNo12).GetCell(7).SetCellValue(weeklyView.projectViews12[i12].ProjectState);
        }
        ISheet sheet13 = hssfworkbook.GetSheet("完成未投运1");
        var rowNo13 = 3;
        for (int i13 = 0; i13 < weeklyView.projectViews13.Count; i13++)
        {
            sheet13.GetRow(i13 + rowNo13).GetCell(0).SetCellValue(weeklyView.projectViews13[i13].CompanyName);
            sheet13.GetRow(i13 + rowNo13).GetCell(1).SetCellValue(i13 + 1);
            sheet13.GetRow(i13 + rowNo13).GetCell(2).SetCellValue(weeklyView.projectViews13[i13].Name);
            sheet13.GetRow(i13 + rowNo13).GetCell(3).SetCellValue(weeklyView.projectViews13[i13].ProjectNatureName);
            sheet13.GetRow(i13 + rowNo13).GetCell(4).SetCellValue(weeklyView.projectViews13[i13].PlanWorkEndTime.ToString());
            sheet13.GetRow(i13 + rowNo13).GetCell(5).SetCellValue(weeklyView.projectViews13[i13].ProjectPolitTime.ToString());
            sheet13.GetRow(i13 + rowNo13).GetCell(6).SetCellValue(weeklyView.projectViews13[i13].ProjectState.ToString());
            sheet13.GetRow(i13 + rowNo13).GetCell(7).SetCellValue(weeklyView.projectViews13[i13].WorkUnit);
            sheet13.GetRow(i13 + rowNo13).GetCell(8).SetCellValue(weeklyView.projectViews13[i13].WorkUnitPMName);
            sheet13.GetRow(i13 + rowNo13).GetCell(9).SetCellValue(weeklyView.projectViews13[i13].SupervisorUnit);
            sheet13.GetRow(i13 + rowNo13).GetCell(10).SetCellValue(weeklyView.projectViews13[i13].SupervisorUnitName);
            sheet13.GetRow(i13 + rowNo13).GetCell(11).SetCellValue(weeklyView.projectViews13[i13].ProjectState);
        }
        ISheet sheet14 = hssfworkbook.GetSheet("在建1");
        var rowNo14 = 3;
        for (int i14 = 0; i14 < weeklyView.projectViews14.Count; i14++)
        {
            sheet14.GetRow(i14 + rowNo14).GetCell(0).SetCellValue(weeklyView.projectViews14[i14].CompanyName);
            sheet14.GetRow(i14 + rowNo14).GetCell(1).SetCellValue(i14 + 1);
            sheet14.GetRow(i14 + rowNo14).GetCell(2).SetCellValue(weeklyView.projectViews14[i14].Name);
            sheet14.GetRow(i14 + rowNo14).GetCell(3).SetCellValue(weeklyView.projectViews14[i14].ProjectNatureName);
            sheet14.GetRow(i14 + rowNo14).GetCell(4).SetCellValue(weeklyView.projectViews14[i14].PlanWorkStartTime.ToString());
            sheet14.GetRow(i14 + rowNo14).GetCell(5).SetCellValue(weeklyView.projectViews14[i14].PlanWorkEndTime.ToString());
            sheet14.GetRow(i14 + rowNo14).GetCell(6).SetCellValue(weeklyView.projectViews14[i14].WorkUnit.ToString());
            sheet14.GetRow(i14 + rowNo14).GetCell(7).SetCellValue(weeklyView.projectViews14[i14].WorkUnitPMName);
            sheet14.GetRow(i14 + rowNo14).GetCell(8).SetCellValue(weeklyView.projectViews14[i14].SupervisorUnit);
            sheet14.GetRow(i14 + rowNo14).GetCell(9).SetCellValue(weeklyView.projectViews14[i14].SupervisorUnitName);
            sheet14.GetRow(i14 + rowNo14).GetCell(10).SetCellValue(weeklyView.projectViews14[i14].ProjectState.ToString());
        }
        ISheet sheet15 = hssfworkbook.GetSheet("正在施工1");
        var rowNo15 = 3;
        for (int i15 = 0; i15 < weeklyView.projectViews16.Count; i15++)
        {
            sheet15.GetRow(i15 + rowNo15).GetCell(0).SetCellValue(weeklyView.projectViews16[i15].CompanyName);
            sheet15.GetRow(i15 + rowNo15).GetCell(1).SetCellValue(i15 + 1);
            sheet15.GetRow(i15 + rowNo15).GetCell(2).SetCellValue(weeklyView.projectViews16[i15].Name);
            sheet15.GetRow(i15 + rowNo15).GetCell(3).SetCellValue(weeklyView.projectViews16[i15].ProjectNatureName);
            sheet15.GetRow(i15 + rowNo15).GetCell(4).SetCellValue(weeklyView.projectViews16[i15].PlanWorkStartTime.ToString());
            sheet15.GetRow(i15 + rowNo15).GetCell(5).SetCellValue(weeklyView.projectViews16[i15].Limit.ToString());
            sheet15.GetRow(i15 + rowNo15).GetCell(6).SetCellValue(weeklyView.projectViews16[i15].ConsumptionPeriod.ToString());
            sheet15.GetRow(i15 + rowNo15).GetCell(7).SetCellValue(weeklyView.projectViews16[i15].SurplusLimit.ToString());
            sheet15.GetRow(i15 + rowNo15).GetCell(8).SetCellValue(weeklyView.projectViews16[i15].WorkSchedule.ToString());
            sheet15.GetRow(i15 + rowNo15).GetCell(9).SetCellValue(weeklyView.projectViews16[i15].WorkUnit);
            sheet15.GetRow(i15 + rowNo15).GetCell(10).SetCellValue(weeklyView.projectViews16[i15].WorkUnitPMName);
            sheet15.GetRow(i15 + rowNo15).GetCell(11).SetCellValue(weeklyView.projectViews16[i15].SupervisorUnit);
            sheet15.GetRow(i15 + rowNo15).GetCell(12).SetCellValue(weeklyView.projectViews16[i15].SupervisorUnitName);
            sheet15.GetRow(i15 + rowNo15).GetCell(13).SetCellValue(weeklyView.projectViews16[i15].ProjectState);
        }

        sheet1.ForceFormulaRecalculation = true;
        sheet2.ForceFormulaRecalculation = true;
        sheet3.ForceFormulaRecalculation = true;
        sheet4.ForceFormulaRecalculation = true;
        sheet5.ForceFormulaRecalculation = true;
        sheet6.ForceFormulaRecalculation = true;
        sheet7.ForceFormulaRecalculation = true;
        sheet8.ForceFormulaRecalculation = true;
        sheet9.ForceFormulaRecalculation = true;
        sheet10.ForceFormulaRecalculation = true;
        sheet11.ForceFormulaRecalculation = true;
        sheet12.ForceFormulaRecalculation = true;
        sheet13.ForceFormulaRecalculation = true;
        sheet14.ForceFormulaRecalculation = true;
        sheet15.ForceFormulaRecalculation = true;
        
        FileStream newfile = new FileStream(pathUrl + weeklyView.Title, FileMode.Create);
        hssfworkbook.Write(newfile);
        newfile.Close();

        return true;
    }

}