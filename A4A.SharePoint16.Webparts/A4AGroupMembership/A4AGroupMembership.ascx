<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="A4AGroupMembership.ascx.cs" Inherits="A4A.SharePoint16.Webparts.A4AGroupMembership.A4AGroupMembership" %>
 
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
#listDataTable th,#listDataTable td {
  text-align:left;
} 
.dataTables_filter {
text-align: left !important;
}
</style>                      

<asp:Label ID="lblMessage" runat="server" />

<table id="listDataTable" class="display" border="0" cellspacing="0" cellpadding="0" style="width:100%;table-layout:fixed;">
    <thead>
        <tr>
             <th>Company</th> 
            <th>Last Name</th> 
            <th>First Name</th> 
            <th style="white-space: nowrap;">Email</th>
            <th>Office Phone</th> 
            <th>Role</th> 
        </tr>
    </thead>
    <tbody> 
        <asp:Repeater runat="server" ID="rep1">
            <ItemTemplate>
                <tr>
                     <td  STYLE="width:18%;">                        
                        <%# DataBinder.Eval(Container.DataItem, "CompanyName") %>
                    </td>
                      <td>
                        <%# DataBinder.Eval(Container.DataItem, "LastName") %>
                    </td>
                      <td >
                        <%# DataBinder.Eval(Container.DataItem, "FirstName") %>
                    </td> 
                    <td class="wrap"  STYLE="width:45%; white-space: nowrap;">
                       <%--  <a href="mailto:"+<%# DataBinder.Eval(Container.DataItem, "Username") %> +" alt=''/> --%>
                        <a href='mailto:<%# DataBinder.Eval(Container.DataItem, "Username") %>' target="_blank">  <%# DataBinder.Eval(Container.DataItem, "Username") %> </a>
                    </td>
                    <td  STYLE="width:8%;">                      
                        <%# DataBinder.Eval(Container.DataItem, "Office") %>                        
                    </td>
                    <td STYLE="width:10%;" >                       
                        <%# DataBinder.Eval(Container.DataItem, "CommitteeRoleName") %>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </tbody> 
</table>

<script>
    $(document).ready(function () {
        var table = $("#listDataTable").DataTable({
            "order": [[0, "asc"]],
            "paging": false,
            "columns": [
                null,
                null,
                null,
                { "width": "35%" },
                null, null]
        }); 
         
    });
</script>
