<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="A4AGroupLiaison.ascx.cs" Inherits="A4A.SharePoint16.Webparts.A4AGroupLiaison.A4AGroupLiaison" %>

<style type="text/css">
    .tdGroupLiaison {
        width:30vw !important; 
    }
    .valigntop{
         vertical-align:text-top !important;
    } 
</style>

<h4><asp:Label ID="lblGroupName" runat="server" Style="font-size: 12px;color: #555;font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif;">LIAISON</asp:Label></h4>
<table>
    <tr>
  <td class="tdGroupLiaison valigntop"><div ID="lblMessage1" runat="server" /></td>
  <td class="tdGroupLiaison valigntop"><div ID="lblMessage2" runat="server" /></td>
  <td class="col-md-6"> </td><tr/>
</table>
