using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using AppsPortal.Library.MimeDetective;
using OfficeOpenXml;

namespace AppsPortal.Extensions
{
    public class FileManager
    {
        public DataSet FillDataSet()
        {
            DataSet ds = new DataSet();
            if (HttpContext.Current.Request.Files["file"].ContentLength > 0)
            {
                ExcelPackage m = new ExcelPackage();


                string fileExtension =
                    System.IO.Path.GetExtension(HttpContext.Current.Request.Files["file"].FileName);
                if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {
                    string fileLocation = HttpContext.Current.Server.MapPath("~/Content/") + HttpContext.Current.Request.Files["file"].FileName;
                    try
                    {
                        if (System.IO.File.Exists(fileLocation))
                        {
                            System.IO.File.Delete(fileLocation);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);

                    }


                    HttpContext.Current.Request.Files["file"].SaveAs(fileLocation);
                    string excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                   ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    //connection String for xls file format.
                    if (fileExtension == ".xls")
                    {
                        excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation +
                                                ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                    }

                    //Create Connection to Excel work book and add oledb namespace
                    OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                    excelConnection.Open();
                    DataTable dt = new DataTable();

                    dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dt == null)
                    {
                        return null;
                    }

                    String[] excelSheets = new String[dt.Rows.Count];
                    int t = 0;
                    //excel data saves in temp file here.
                    foreach (DataRow row in dt.Rows)
                    {
                        excelSheets[t] = row["TABLE_NAME"].ToString();
                        t++;
                    }

                    OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);
                    string query = string.Format("Select * from [{0}]", excelSheets[0]);
                    using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                    {
                        dataAdapter.Fill(ds);
                    }
                }
            }


            return ds;
        }

        public DataTable ToDataTable(ExcelPackage package)
        {
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            DataTable table = new DataTable();
            foreach (var firstRowCell in workSheet.Cells[1, 1, 1, workSheet.Dimension.End.Column])
            {
                table.Columns.Add(firstRowCell.Text);
            }

            for (var rowNumber = 2; rowNumber <= workSheet.Dimension.End.Row; rowNumber++)
            {
                var row = workSheet.Cells[rowNumber, 1, rowNumber, workSheet.Dimension.End.Column];
                var newRow = table.NewRow();
                foreach (var cell in row)
                {
                    newRow[cell.Start.Column - 1] = cell.Text;
                }

                table.Rows.Add(newRow);
            }

            return table;
        }

        public DataTable ImportDataSet()
        {
            DataTable dt = new DataTable();
            if (HttpContext.Current.Request.Files["file"].ContentLength > 0)
            {
                string fileExtension =
                    System.IO.Path.GetExtension(HttpContext.Current.Request.Files["file"].FileName);
                if (FileTypeValidator.IsExcel(HttpContext.Current.Request.Files["file"].InputStream))
                {
                    string fileLocation = HttpContext.Current.Server.MapPath("~/Uploads/OVS/Temp/") + HttpContext.Current.Request.Files["file"].FileName;
                    try
                    {
                        if (System.IO.File.Exists(fileLocation))
                        {
                            System.IO.File.Delete(fileLocation);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);

                    }
                    
                    HttpContext.Current.Request.Files["file"].SaveAs(fileLocation);
                    
                    var excel = new ExcelPackage(new FileInfo(fileLocation));
                    dt = ToDataTable(excel);
                }
            }


            return dt;
        }


    }
}