<%@ Assembly Name="A4A.SharePoint16.Webparts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=1f1f1f331334862a" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MySites.ascx.cs" Inherits="A4A.SharePoint16.Webparts.ControlTemplates.A4A.SharePoint16.Webparts.MySites" %>
	
<style type="text/css">
#ctl00_Sitemaps_ddlMySites, #ddlMySites
	 {
	    position: relative !important; 
	    margin-top: -2px!important;
        margin-left: 1em !important; 
        color:#0072BC; 
        font-family: 'Helvetica Neue', Arial, sans-serif; text-transform: uppercase; 
        width: 220px;
	}  
	#DeltaTopNavigation{ 
          float:left!important;
        }
</style>
<script type="text/javascript">

function GotoSite()
{
    var destination = document.getElementById("ctl00_Sitemaps_ddlMySites").value;
    var selected = document.getElementById("ctl00_Sitemaps_ddlMySites").text;
    var version = navigator.appVersion;
    // sets variable == browser version
    if(destination!=="no_page")
    {
      if (version.indexOf("MSIE") >= -1)
      // checks to see if using IE
      {
         window.location.href=destination;
      }
      else
      {
         window.open(destination, target=="_self");
      }
    }
}

</script> 
<asp:DropDownList ID="ddlMySites"  runat="server" name="mysites" onChange="javascript: GotoSite();"></asp:DropDownList>
<asp:Label ID="lblMessage" runat="server"></asp:Label>

