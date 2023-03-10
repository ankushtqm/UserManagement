using System;
using System.Collections.Generic; 
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Security;
using System.Data;

namespace ATA.CodeLibrary
{
    public class DataViewToExcelExporter
    {
        const string SEPARATOR = ","; //could use "," or "\t"


        public static void ExportToExcel(HttpResponse response, DataView dv, string filename)
        {
            ExporterUtil.ClearResponse(response);
            ExporterUtil.AddAttachmentHeader(response, filename + ".csv");//.csv .xls  
            response.ContentType = "application/vnd.ms-excel";//"application/vnd.ms-excel" "application/ms-excel"; "Application/x-msexcel";

            DataTable table = dv.Table;
            string currentSeparator = "";
            foreach (DataColumn dc in table.Columns)
            {
                response.Write(currentSeparator + dc.ColumnName.Replace(SEPARATOR, " "));
                currentSeparator = SEPARATOR;
            }
            //append a carriage return 
            response.Write("\r\n");

            //loop thru each row of our datatable
            foreach (DataRowView row in dv)
            {
                currentSeparator = "";
                //loop thru each column in our datatable
                foreach (DataColumn column in table.Columns)
                {
                    //get the value for tht row on the specified column
                    // and append our separator 
                    response.Write(currentSeparator + Convert.ToString(row[column.ColumnName]).Replace(SEPARATOR, " ").Replace("\r\n", " "));

                    currentSeparator = SEPARATOR;
                }
                //append a carriage return
                response.Write("\r\n");
            }
            ExporterUtil.FlushAndEndResponse(response);
        }

        public static HttpResponse ExportToExcelResponse(HttpResponse response, DataView dv, string filename)
        {
            ExporterUtil.ClearResponse(response);
            ExporterUtil.AddAttachmentHeader(response, filename + ".csv");//.csv .xls  
            response.ContentType = "application/vnd.ms-excel";//"application/vnd.ms-excel" "application/ms-excel"; "Application/x-msexcel";

            DataTable table = dv.Table;
            string currentSeparator = "";
            foreach (DataColumn dc in table.Columns)
            {
                response.Write(currentSeparator + dc.ColumnName.Replace(SEPARATOR, " "));
                currentSeparator = SEPARATOR;
            }
            //append a carriage return 
            response.Write("\r\n");

            //loop thru each row of our datatable
            foreach (DataRowView row in dv)
            {
                currentSeparator = "";
                //loop thru each column in our datatable
                foreach (DataColumn column in table.Columns)
                {
                    //get the value for tht row on the specified column
                    // and append our separator 
                    response.Write(currentSeparator + Convert.ToString(row[column.ColumnName]).Replace(SEPARATOR, " ").Replace("\r\n", " "));

                    currentSeparator = SEPARATOR;
                }
                //append a carriage return
                response.Write("\r\n");
            }
            return response;
        }
    }
} 
