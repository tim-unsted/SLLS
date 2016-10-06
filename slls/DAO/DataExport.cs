using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Web;
using OfficeOpenXml;

namespace slls.DAO
{
    public class DataExport
    {
        private readonly Type _entityType;

        public DataExport(Type entityType)
        {
            _entityType = entityType;
        }

        public static void WriteTsv<TEntityType>(IEnumerable<TEntityType> data, TextWriter output) where TEntityType : class
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(TEntityType));
            foreach (PropertyDescriptor prop in props)
            {
                output.Write(prop.DisplayName); // header
                output.Write("\t");
            }
            output.WriteLine();
            foreach (TEntityType item in data)
            {
                foreach (PropertyDescriptor prop in props)
                {
                    if (prop.Converter != null)
                        output.Write(prop.Converter.ConvertToString(
                            prop.GetValue(item)));
                    output.Write("\t");
                }
                output.WriteLine();
            }
        }

        public static void ExportExcel97<TEntityType>(IEnumerable<TEntityType> data) where TEntityType : class
        {
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=MyExport.xls");
            HttpContext.Current.Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            WriteTsv(data, HttpContext.Current.Response.Output);
            HttpContext.Current.Response.End();
        }

        public static void ExportExcel2007<TEntityType>(IEnumerable<TEntityType> data) where TEntityType : class
        {
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
            workSheet.Cells[1, 1].LoadFromCollection(data, true);
            using (var memoryStream = new MemoryStream())
            {
                HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment;  filename=MyExport.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.End();
            }
        }
    }
}