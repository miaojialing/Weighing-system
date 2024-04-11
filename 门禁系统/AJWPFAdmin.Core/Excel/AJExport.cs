using AJWPFAdmin.Core.Models;
using AJWPFAdmin.Core.Utils;
using ClosedXML.Excel;
using ClosedXML.Excel.Drawings;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Masuit.Tools;
using Masuit.Tools.Reflection;
using Masuit.Tools.Systems;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AJWPFAdmin.Core.Excel
{
    public class AJExport
    {
        private static IOrderedEnumerable<AJExportItem> GetRow<T>(T item)
        {
            var result = new List<AJExportItem>();

            var props = item.GetType().GetRuntimeProperties();

            foreach (var prop in props)
            {
                var attr = prop.GetCustomAttribute<AJExportFieldAttribute>();
                if (attr == null)
                {
                    continue;
                }

                if (!attr.Dynamic)
                {
                    var val = prop.GetValue(item);

                    if (prop.PropertyType.IsEnum)
                    {
                        val = val.ToString();
                    }

                    result.Add(new AJExportItem
                    {
                        Title = attr.ColumnName,
                        Value = val,
                        Index = attr.ColumnIndex,
                        IsImage = attr.IsImage,
                    });
                }
                else
                {
                    if (prop.GetValue(item) is not Dictionary<string, string> dic)
                    {
                        continue;
                    }

                    var dicIndex = 0;
                    foreach (var dicItem in dic)
                    {
                        result.Add(new AJExportItem
                        {
                            Title = dicItem.Key,
                            Value = dicItem.Value,
                            Index = attr.ColumnIndex + dicIndex,
                            Dynamic = true,
                            IsImage = attr.IsImage,
                        });
                        dicIndex++;
                    }
                }
            }

            return result.OrderBy(p => p.Index);
        }

        public static ProcessResult CreateExcel<T>(IList<T> source,
            string filePath,
            string fileName = "export",
            bool rawRows = false) where T : class, new()
        {
            var result = new ProcessResult();

            if (source == null || source.Count == 0)
            {
                result.SetError("没有任何数据导出");
                return result;
            }

            XLWorkbook wb = null;
            IXLWorksheet ws = null;

            var rawRowList = new List<IList<AJExportItem>>();

            if (!rawRows)
            {
                wb = new XLWorkbook();
                ws = wb.Worksheets.Add();
            }

            foreach (var item in source)
            {
                rawRowList.Add(GetRow(item).ToList());
            }

            var headers = rawRowList.SelectMany(p => p).Select(p => p.Title).Distinct().ToArray();

            if (rawRows)
            {
                result.SetSuccess(rawRowList);
                return result;
            }

            var headerLen = headers.Length;
            for (int i = 0; i < headerLen; i++)
            {
                ws.Cell(1, i + 1).SetValue(headers[i]);
            }

            // 循环行
            var rowIndex = 2;
            foreach (var row in rawRowList)
            {
                var index = 1;
                foreach (var header in headers)
                {
                    var rowVal = row.FirstOrDefault(p => p.Title == header);
                    if (rowVal == null)
                    {
                        continue;
                    }

                    IXLCell cell;

                    if (rowVal.IsImage)
                    {
                        var file = rowVal.Value?.ToString() ?? "";

                        if (file.StartsWith("["))
                        {
                            file = CommonUtil.TryGetJSONObject<string[]>(file)?.ElementAtOrDefault(0);
                        }

                        if (string.IsNullOrEmpty(file))
                        {
                            cell = ws.Cell(rowIndex, index).SetValue("暂无图片");
                        }
                        else if (!File.Exists(file))
                        {
                            cell = ws.Cell(rowIndex, index).SetValue("图片不存在");
                        }
                        else
                        {
                            cell = ws.Cell(rowIndex, index);
                            ws.Row(rowIndex).Height = 80;
                            ws.Column(index).Width = 80;
                            var fs = File.OpenRead(file);
                            var img = System.Drawing.Image.FromStream(fs, false, false);

                            var factor = (double)img.Width / 160d / 100d;

                            fs.Dispose();
                            img.Dispose();

                            ws.AddPicture(file).MoveTo(cell).ScaleHeight(factor, true).ScaleWidth(factor, true)
                                .WithPlacement(XLPicturePlacement.Move);

                            cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        }
                    }
                    else
                    {
                        
                        if (rowVal.Value != null)
                        {
                            if (rowVal.Value is int || rowVal.Value is int?)
                            {
                                cell = ws.Cell(rowIndex, index).SetValue((int?)rowVal.Value);
                                continue;
                            }
                            if (rowVal.Value is short || rowVal.Value is short?)
                            {
                                cell = ws.Cell(rowIndex, index).SetValue((short?)rowVal.Value);
                                continue;
                            }
                            if (rowVal.Value is DateTime || rowVal.Value is DateTime?)
                            {
                                cell = ws.Cell(rowIndex, index).SetValue((DateTime?)rowVal.Value);
                                continue;
                            }

                            if (rowVal.Value is bool || rowVal.Value is bool?)
                            {
                                var s = ((bool?)rowVal.Value).GetValueOrDefault() ? "是" : "否";
                                cell = ws.Cell(rowIndex, index).SetValue(s);
                                continue;
                            }

                            cell = ws.Cell(rowIndex, index).SetValue(rowVal.Value.ToString());
                        }
                        else
                        {
                            cell = ws.Cell(rowIndex, index).SetValue("");
                        }
                    }

                    cell?.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                    index++;
                }
                rowIndex++;
            }

            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    filePath = "export/data";
                }

                if (string.IsNullOrWhiteSpace(fileName))
                {
                    fileName = $"export_{SnowFlake.GetInstance().GetUniqueId()}";
                }

                ws.Columns().AdjustToContents(24d, 64d);
                wb.SaveAs(filePath);

                result.SetSuccess(filePath);
            }
            catch (Exception e)
            {
                result.SetError($"导出Excel文件失败:{e.Message}");
            }

            return result;
        }
    }
}
