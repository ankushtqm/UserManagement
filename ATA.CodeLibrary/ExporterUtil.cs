 using System;
    using System.Collections.Generic;
    using System.Text;
 using System.Web;

namespace ATA.CodeLibrary
{ 
     public class ExporterUtil
        {
            public static void ClearResponse(HttpResponse response)
            {
                response.Clear();
                response.ClearHeaders();
                response.ClearContent();
            }

            public static void FlushAndEndResponse(HttpResponse response)
            {
                response.Flush();
                response.End();
            }

            public static void AddAttachmentHeader(HttpResponse response, string fileName)
            {
                response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                //"content-disposition" 
            }
        }
    }

 
