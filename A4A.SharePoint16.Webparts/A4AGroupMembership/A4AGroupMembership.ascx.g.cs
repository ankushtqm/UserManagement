﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace A4A.SharePoint16.Webparts.A4AGroupMembership {
    using System.Web.UI.WebControls.Expressions;
    using System.Web.UI.HtmlControls;
    using System.Collections;
    using System.Text;
    using System.Web.UI;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.SharePoint.WebPartPages;
    using System.Web.SessionState;
    using System.Configuration;
    using Microsoft.SharePoint;
    using System.Web;
    using System.Web.DynamicData;
    using System.Web.Caching;
    using System.Web.Profile;
    using System.ComponentModel.DataAnnotations;
    using System.Web.UI.WebControls;
    using System.Web.Security;
    using System;
    using Microsoft.SharePoint.Utilities;
    using System.Text.RegularExpressions;
    using System.Collections.Specialized;
    using System.Web.UI.WebControls.WebParts;
    using Microsoft.SharePoint.WebControls;
    using System.CodeDom.Compiler;
    
    
    public partial class A4AGroupMembership {
        
        [GeneratedCodeAttribute("Microsoft.VisualStudio.SharePoint.ProjectExtensions.CodeGenerators.SharePointWebP" +
            "artCodeGenerator", "15.0.0.0")]
        protected global::System.Web.UI.WebControls.Label lblMessage;
        
        [GeneratedCodeAttribute("Microsoft.VisualStudio.SharePoint.ProjectExtensions.CodeGenerators.SharePointWebP" +
            "artCodeGenerator", "15.0.0.0")]
        protected global::System.Web.UI.WebControls.Repeater rep1;
        
        [GeneratedCodeAttribute("Microsoft.VisualStudio.SharePoint.ProjectExtensions.CodeGenerators.SharePointWebPartCodeGenerator", "15.0.0.0")]
        public static implicit operator global::System.Web.UI.TemplateControl(A4AGroupMembership target) 
        {
            return target == null ? null : target.TemplateControl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        [GeneratedCodeAttribute("Microsoft.VisualStudio.SharePoint.ProjectExtensions.CodeGenerators.SharePointWebP" +
            "artCodeGenerator", "15.0.0.0")]
        private global::System.Web.UI.WebControls.Label @__BuildControllblMessage() {
            global::System.Web.UI.WebControls.Label @__ctrl;
            @__ctrl = new global::System.Web.UI.WebControls.Label();
            this.lblMessage = @__ctrl;
            @__ctrl.ApplyStyleSheetSkin(this.Page);
            @__ctrl.ID = "lblMessage";
            return @__ctrl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        [GeneratedCodeAttribute("Microsoft.VisualStudio.SharePoint.ProjectExtensions.CodeGenerators.SharePointWebP" +
            "artCodeGenerator", "15.0.0.0")]
        private global::System.Web.UI.DataBoundLiteralControl @__BuildControl__control3() {
            global::System.Web.UI.DataBoundLiteralControl @__ctrl;
            @__ctrl = new global::System.Web.UI.DataBoundLiteralControl(8, 7);
            @__ctrl.TemplateControl = this;
            @__ctrl.SetStaticString(0, "\r\n                <tr>\r\n                     <td  STYLE=\"width:18%;\">            " +
                    "            \r\n                        ");
            @__ctrl.SetStaticString(1, "\r\n                    </td>\r\n                      <td>\r\n                        " +
                    "");
            @__ctrl.SetStaticString(2, "\r\n                    </td>\r\n                      <td >\r\n                       " +
                    " ");
            @__ctrl.SetStaticString(3, "\r\n                    </td> \r\n                    <td class=\"wrap\"  STYLE=\"width:" +
                    "45%; white-space: nowrap;\">\r\n                       \r\n                        <a" +
                    " href=\'mailto:");
            @__ctrl.SetStaticString(4, "\' target=\"_blank\">  ");
            @__ctrl.SetStaticString(5, " </a>\r\n                    </td>\r\n                    <td  STYLE=\"width:8%;\">    " +
                    "                  \r\n                        ");
            @__ctrl.SetStaticString(6, "                        \r\n                    </td>\r\n                    <td STYL" +
                    "E=\"width:10%;\" >                       \r\n                        ");
            @__ctrl.SetStaticString(7, "\r\n                    </td>\r\n                </tr>\r\n            ");
            @__ctrl.DataBinding += new System.EventHandler(this.@__DataBind__control3);
            return @__ctrl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        [GeneratedCodeAttribute("Microsoft.VisualStudio.SharePoint.ProjectExtensions.CodeGenerators.SharePointWebP" +
            "artCodeGenerator", "15.0.0.0")]
        public void @__DataBind__control3(object sender, System.EventArgs e) {
            System.Web.UI.WebControls.RepeaterItem Container;
            System.Web.UI.DataBoundLiteralControl target;
            target = ((System.Web.UI.DataBoundLiteralControl)(sender));
            Container = ((System.Web.UI.WebControls.RepeaterItem)(target.BindingContainer));
            target.SetDataBoundString(0, global::System.Convert.ToString(DataBinder.Eval(Container.DataItem, "CompanyName"), global::System.Globalization.CultureInfo.CurrentCulture));
            target.SetDataBoundString(1, global::System.Convert.ToString(DataBinder.Eval(Container.DataItem, "LastName"), global::System.Globalization.CultureInfo.CurrentCulture));
            target.SetDataBoundString(2, global::System.Convert.ToString(DataBinder.Eval(Container.DataItem, "FirstName"), global::System.Globalization.CultureInfo.CurrentCulture));
            target.SetDataBoundString(3, global::System.Convert.ToString(DataBinder.Eval(Container.DataItem, "Username"), global::System.Globalization.CultureInfo.CurrentCulture));
            target.SetDataBoundString(4, global::System.Convert.ToString(DataBinder.Eval(Container.DataItem, "Username"), global::System.Globalization.CultureInfo.CurrentCulture));
            target.SetDataBoundString(5, global::System.Convert.ToString(DataBinder.Eval(Container.DataItem, "Office"), global::System.Globalization.CultureInfo.CurrentCulture));
            target.SetDataBoundString(6, global::System.Convert.ToString(DataBinder.Eval(Container.DataItem, "CommitteeRoleName"), global::System.Globalization.CultureInfo.CurrentCulture));
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        [GeneratedCodeAttribute("Microsoft.VisualStudio.SharePoint.ProjectExtensions.CodeGenerators.SharePointWebP" +
            "artCodeGenerator", "15.0.0.0")]
        private void @__BuildControl__control2(System.Web.UI.Control @__ctrl) {
            global::System.Web.UI.DataBoundLiteralControl @__ctrl1;
            @__ctrl1 = this.@__BuildControl__control3();
            System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
            @__parser.AddParsedSubObject(@__ctrl1);
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        [GeneratedCodeAttribute("Microsoft.VisualStudio.SharePoint.ProjectExtensions.CodeGenerators.SharePointWebP" +
            "artCodeGenerator", "15.0.0.0")]
        private global::System.Web.UI.WebControls.Repeater @__BuildControlrep1() {
            global::System.Web.UI.WebControls.Repeater @__ctrl;
            @__ctrl = new global::System.Web.UI.WebControls.Repeater();
            this.rep1 = @__ctrl;
            @__ctrl.ItemTemplate = new System.Web.UI.CompiledTemplateBuilder(new System.Web.UI.BuildTemplateMethod(this.@__BuildControl__control2));
            @__ctrl.ID = "rep1";
            return @__ctrl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        [GeneratedCodeAttribute("Microsoft.VisualStudio.SharePoint.ProjectExtensions.CodeGenerators.SharePointWebP" +
            "artCodeGenerator", "15.0.0.0")]
        private void @__BuildControlTree(global::A4A.SharePoint16.Webparts.A4AGroupMembership.A4AGroupMembership @__ctrl) {
            System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
            @__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n \r\n<link rel=\"stylesheet\" type=\"text/css\" href=\"https://cdn.datatables.net/1.10" +
                        ".20/css/jquery.dataTables.min.css\">\r\n<script type=\"text/javascript\" src=\"https:/" +
                        "/cdn.datatables.net/1.10.20/js/jquery.dataTables.min.js\"></script> \r\n<link rel=\"" +
                        "stylesheet\" href=\"https://code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui." +
                        "css\">\r\n<script src=\"https://code.jquery.com/ui/1.11.4/jquery-ui.js\"></script>\r\n\r" +
                        "\n\r\n<style type=\"text/css\" class=\"init\">\r\n\r\n   td.details-control {\r\n   backgroun" +
                        "d: url(\'/_catalogs/masterpage/FuelPortal/img/details_open.png\') no-repeat center" +
                        " center;\r\n   cursor: pointer;\r\n   }\r\n   tr.shown td.details-control {\r\n   backgr" +
                        "ound: url(\'/_catalogs/masterpage/FuelPortal/img/details_close.png\') no-repeat ce" +
                        "nter center;\r\n   } \r\n\r\n</style>\r\n\r\n<!--/****************************************" +
                        "********************************************************************************" +
                        "*******/\r\n   /** Code form UI tool ***********/\r\n   /***************************" +
                        "********************************/-->\r\n<!--  Bootstrap css  -->\r\n<style type=\"tex" +
                        "t/css\" media=\"all\">\r\n   .top-buffer { margin-top:10px; }\r\n     .bot-buffer { mar" +
                        "gin-bottom:10px; margin-right: 10px;}\r\n   .left {float:left !important;}\r\n   .ri" +
                        "ght {float:right !important;}\r\n   .filterlabels{float:left; padding-right: 5px;p" +
                        "adding-top: 3px;}\r\n   .article {\r\n        margin-left:20px;\r\n       overflow:vis" +
                        "ible !important;\r\n    }\r\n       h2 {\r\n    margin-top:0px;\r\n    margin-bottom: 0p" +
                        "x;\r\n    }\r\n   table.dataTable td { \r\n    word-wrap: break-word;\r\n} \r\n.dataTables" +
                        "_wrapper {\r\n    margin-top: 2em !important;\r\n}\r\n#listDataTable th,#listDataTable" +
                        " td {\r\n  text-align:left;\r\n} \r\n.dataTables_filter {\r\ntext-align: left !important" +
                        ";\r\n}\r\n</style>                      \r\n\r\n"));
            global::System.Web.UI.WebControls.Label @__ctrl1;
            @__ctrl1 = this.@__BuildControllblMessage();
            @__parser.AddParsedSubObject(@__ctrl1);
            @__parser.AddParsedSubObject(new System.Web.UI.LiteralControl(@"

<table id=""listDataTable"" class=""display"" border=""0"" cellspacing=""0"" cellpadding=""0"" style=""width:100%;table-layout:fixed;"">
    <thead>
        <tr>
             <th>Company</th> 
            <th>Last Name</th> 
            <th>First Name</th> 
            <th style=""white-space: nowrap;"">Email</th>
            <th>Office Phone</th> 
            <th>Role</th> 
        </tr>
    </thead>
    <tbody> 
        "));
            global::System.Web.UI.WebControls.Repeater @__ctrl2;
            @__ctrl2 = this.@__BuildControlrep1();
            @__parser.AddParsedSubObject(@__ctrl2);
            @__parser.AddParsedSubObject(new System.Web.UI.LiteralControl(@"
    </tbody> 
</table>

<script>
    $(document).ready(function () {
        var table = $(""#listDataTable"").DataTable({
            ""order"": [[0, ""asc""]],
            ""paging"": false,
            ""columns"": [
                null,
                null,
                null,
                { ""width"": ""35%"" },
                null, null]
        }); 
         
    });
</script>
"));
        }
        
        [GeneratedCodeAttribute("Microsoft.VisualStudio.SharePoint.ProjectExtensions.CodeGenerators.SharePointWebP" +
            "artCodeGenerator", "15.0.0.0")]
        private void InitializeControl() {
            this.@__BuildControlTree(this);
            this.Load += new global::System.EventHandler(this.Page_Load);
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        [GeneratedCodeAttribute("Microsoft.VisualStudio.SharePoint.ProjectExtensions.CodeGenerators.SharePointWebP" +
            "artCodeGenerator", "15.0.0.0")]
        protected virtual object Eval(string expression) {
            return global::System.Web.UI.DataBinder.Eval(this.Page.GetDataItem(), expression);
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        [GeneratedCodeAttribute("Microsoft.VisualStudio.SharePoint.ProjectExtensions.CodeGenerators.SharePointWebP" +
            "artCodeGenerator", "15.0.0.0")]
        protected virtual string Eval(string expression, string format) {
            return global::System.Web.UI.DataBinder.Eval(this.Page.GetDataItem(), expression, format);
        }
    }
}
