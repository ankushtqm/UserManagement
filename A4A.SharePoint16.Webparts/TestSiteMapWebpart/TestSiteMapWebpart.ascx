<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TestSiteMapWebpart.ascx.cs" Inherits="A4A.SharePoint16.Webparts.TestSiteMapWebpart.TestSiteMapWebpart" %>

<link rel="stylesheet" type="text/css" href="https://cdn.datatables.net/1.10.20/css/jquery.dataTables.min.css">
<script type="text/javascript" src="https://cdn.datatables.net/1.10.20/js/jquery.dataTables.min.js"></script> 
<link rel="stylesheet" href="https://code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css">
<script src="https://code.jquery.com/ui/1.11.4/jquery-ui.js"></script>
 
<style type="text/css" class="init">
   td.details-control {
   background: url('/_catalogs/masterpage/FuelPortal/img/details_open.png') no-repeat center center;
   cursor: pointer;
   }
   tr.shown td.details-control {
   background: url('/_catalogs/masterpage/FuelPortal/img/details_close.png') no-repeat center center;
   } 

</style>

<!--/*******************************************************************************************************************************/
   /** Code form UI tool ***********/
   /***********************************************************/-->
<!--  Bootstrap css  -->
<style type="text/css" media="all">
   .top-buffer { margin-top:10px; }
     .bot-buffer { margin-bottom:10px; margin-right: 10px;}
   .left {float:left !important;}
   .right {float:right !important;}
   .filterlabels{float:left; padding-right: 5px;padding-top: 3px;}
   .article {
        margin-left:20px;
       overflow:visible !important;
    }
       h2 {
    margin-top:0px;
    margin-bottom: 0px;
    }
   table.dataTable td { 
    word-wrap: break-word;
} 
    .dataTables_wrapper {
    margin-top: 2em !important;
    }   
</style>                      

<asp:Label ID="lblMessage" runat="server" />

<table id="SiteMapDT" class="display" border="0" cellspacing="0" cellpadding="0">
    <thead>
        <tr>
             <th>Group</th> 
            <th>Liaison</th> 
            
        </tr>
    </thead>
    <tbody> 
        <asp:Repeater runat="server" ID="rep1">
            <ItemTemplate>
                <tr>
                     <td>                        
                        <a href='<%# DataBinder.Eval(Container.DataItem,"GroupSiteURL") %>'> <%# DataBinder.Eval(Container.DataItem, "GroupName") %> </a>
                    </td>
                      <td>
                          <%# DataBinder.Eval(Container.DataItem, "Liaison1") %>                        
                    </td>
                    <%--<td>
                           <%# DataBinder.Eval(Container.DataItem, "Liaison2") %>  
                    </td>  --%>
                </tr>
                
            </ItemTemplate>
        </asp:Repeater>
    </tbody> 
</table>

<%--<script type="text/javascript">
    var j = jQuery.noConflict();
</script>--%>

<script>

    //Setting the search for datatable
    $('#SiteMapDT thead th').each(function () {
        var title = $('#SiteMapDT thead th').eq($(this).index()).text();
        if (title.toLowerCase().indexOf("detail") < 0 && title.toLowerCase().indexOf("edit") < 0) {
            $(this).html('<input type="text" placeholder="Search ' + title + '" style="float:left;" />');
        }
        else {
            $(this).html('');
        }
    });

    $(document).ready(function () {
     
        //Setting the DataTable -  // DataTable
        var table = $("#SiteMapDT").DataTable({
            "paging": false, 
            "columns": [
                null,
                null]
        });
         

        // Apply the search
        table.columns().eq(0).each(function (colIdx) {
            
            $('input', table.column(colIdx).header()).on('keyup change', function () {
                table
                    .column(colIdx)
                    .search(this.value)
                    .draw();
            });
        });
          
    });
</script>



<%--
/*
<asp:Label ID="lblMessage" runat="server"></asp:Label>
<div id="LoggedinUser" runat="server"></div>
<asp:DropDownList ID="ddlMySites"  runat="server"></asp:DropDownList>

*/--%>