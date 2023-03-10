using System;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using System.Collections.Generic;

namespace A4A.SharePoint16.Webparts.A4AListView
{
    [ToolboxItemAttribute(false)]
    public partial class A4AListView : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public A4AListView()
        {
        }


        [Personalizable(PersonalizationScope.Shared), WebBrowsable(true),
           WebDisplayNameAttribute("Site URL"), WebDescription("Enter the URL of the site you want to choose the list from:"), CategoryAttribute("A4A WebPart Settings")
       ]
        public String SiteURL
        {
            get;
            set;
        }
        [Personalizable(PersonalizationScope.Shared), WebBrowsable(true),
          WebDisplayNameAttribute("Subsite URL"), WebDescription("Enter the relative url of the site you want to choose the list from:"), CategoryAttribute("A4A WebPart Settings")
      ]
        public String SubsiteURL
        {
            get;
            set;
        }
        [Personalizable(PersonalizationScope.Shared), WebBrowsable(true),
            WebDisplayNameAttribute("List Name"), WebDescription("Enter the list name you want to display:"), CategoryAttribute("A4A WebPart Settings")
        ]
        public String ListName
        {
            get;
            set;
        }

        [Personalizable(PersonalizationScope.Shared), WebBrowsable(true),
           WebDisplayNameAttribute("View Name"), WebDescription("Enter the View name:"), CategoryAttribute("A4A WebPart Settings")
       ]
        public String ViewName
        {
            get;
            set;
        }

        [Personalizable(PersonalizationScope.Shared), WebBrowsable(true),
           WebDisplayNameAttribute("Column 1"), WebDescription("Column 1:"), CategoryAttribute("A4A WebPart Settings")
       ]
        public String Column1
        {
            get;
            set;
        }

        [Personalizable(PersonalizationScope.Shared), WebBrowsable(true),
           WebDisplayNameAttribute("Column 2"), WebDescription("Column 2:"), CategoryAttribute("A4A WebPart Settings")
       ]
        public String Column2
        {
            get;
            set;
        }

        [Personalizable(PersonalizationScope.Shared), WebBrowsable(true),
          WebDisplayNameAttribute("Column 3"), WebDescription("Column 3:"), CategoryAttribute("A4A WebPart Settings")
      ]
        public String Column3
        {
            get;
            set;
        }
        [Personalizable(PersonalizationScope.Shared), WebBrowsable(true), WebDisplayNameAttribute("Item Limit"), WebDescription("Item Count:"),
           CategoryAttribute("A4A WebPart Settings")]
        public int ItemCount
        {
            get;
            set;
        }
        [Personalizable(PersonalizationScope.Shared), WebBrowsable(true), WebDisplayNameAttribute("More Link"), WebDescription("More Link:"),
           CategoryAttribute("A4A WebPart Settings")]
        public String MoreLink
        {
            get;
            set;
        }
        [Personalizable(PersonalizationScope.Shared), WebBrowsable(true), WebDisplayNameAttribute("Fields List/Column Names"), WebDescription("Information:"), 
            CategoryAttribute("A4A WebPart Settings")]
        public String FieldsList
        {
            get;
            set;
        }



        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        

        protected void Page_Load(object sender, EventArgs e)
        {
            int i = 0;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            this.Title = this.Title.Length > 20 ? this.Title.Substring(0, 20) + "..." : this.Title;
            this.TitleUrl = this.MoreLink;

            try
            {
                // Starting with ClientContext, the constructor requires a URL to the
                // server running SharePoint.
                ItemCount = (ItemCount > 0) ? ItemCount : 5;

                if (!string.IsNullOrEmpty(SiteURL))
                { 
                    i = 1;
                   

                    ClientContext _clientContext = new ClientContext(SiteURL);
                    ItemCount = (ItemCount > 0) ? ItemCount : 5;
                    using (SPSite site = new SPSite(SiteURL))
                    {
                        using (SPWeb web = site.OpenWeb(SubsiteURL))
                        {
                            SPList list = web.Lists.TryGetList(this.ListName); 
                            i = 6; 
                            if (list != null)
                            {
                                i = 7;
                                SPView view = list.Views[ViewName];   //custom view name
                                SPListTemplateType lsttmptype = list.BaseTemplate;
                                i = 8;                                 
                                SPQuery query = new SPQuery();
                                string tmp = view.Query;
                                query.RowLimit = (uint)ItemCount;
                                 
                                query.Query = tmp;
                                SPListItemCollection items = list.GetItems(query);
                                 
                               // Get a collection of view field names. 
                               StringCollection viewFields = view.ViewFields.ToStringCollection();
                                i = 9;
                                int cnt = 0;
                                i = 10;
                                List<string> viewfld = new List<string>();
                                
                                foreach (string f  in view.ViewFields)
                                {
                                    cnt++;
                                    viewfld.Add(f);
                                    i = 14;
                                    SPField fld = list.Fields.GetFieldByInternalName(f);
                                    i = 141;
                                    SPFieldType fType = fld.Type;
                                    i = 15;
                                    string fTypeName = fld.TypeDisplayName;                                                                        
                                    i = 16;
                                    sb.Append(cnt +". "+ f  + "("+fType+")");
                                }
                                i = 17;
                                //this.FieldsList = sb.ToString();
                                sb.Clear(); 
                                /////Print data for each item in the view.
                                sb.AppendLine("<div class='a4awpheight'>");
                                i = 171;
                                StringBuilder displaylist = new StringBuilder();
                                
                                foreach (SPField str in items.Fields)
                                {
                                    i +=1;
                                    if(viewfld.Contains(str.InternalName)) //Add only if the fields contain in the view
                                    displaylist.Append("Title:" + str.Title + " & InternalName:" + str.InternalName + "---");//+ "Id:  " + str.Id + " FieldValueType: " + str.FieldValueType + "StaticName:" + str.StaticName );

                                }
                                i = 1800;
                                this.FieldsList = displaylist.ToString();
                                sb.Append(BuildtheContent(items, lsttmptype));
                                #region old logic
                                //foreach (SPListItem item in items)
                                //{
                                //    sb.AppendLine("<br> ");
                                //    sb.AppendLine("<div>");                                    
                                //    i = 10;
                                //    try
                                //    {
                                //        if (!string.IsNullOrEmpty(this.Column1) &&  item.Fields.ContainsField(this.Column1) && item[this.Column1] != null)
                                //        {
                                //            //sb.AppendLine(item.Fields[this.Column1].Type.ToString());
                                //            if (item.Fields[this.Column1].Type == SPFieldType.DateTime)
                                //            {
                                //                DateTime dt;
                                //                DateTime.TryParse(item[this.Column1].ToString(), out dt);
                                //                sb.AppendLine(dt.Date.ToString("yyyy-MM-dd"));
                                //            }

                                //            else
                                //            if (item.Fields[this.Column1].Type == SPFieldType.URL || item.Fields[this.Column1].Type == SPFieldType.File)
                                //            {
                                //                SPFieldUrlValue fieldValue = new SPFieldUrlValue(item[this.Column1].ToString());

                                //                string linkTitle = fieldValue.Description;

                                //                string linkUrl = fieldValue.Url;

                                //                sb.AppendLine("<a href ='" + linkUrl + "' >" + linkTitle + "</a>");
                                //            }
                                //            else
                                //            if (item.Fields[this.Column1].Type == SPFieldType.Text)
                                //            {
                                //                int len = item.Fields[this.Column1].ToString().Length > 40 ? 40 : item[this.Column1].ToString().Length;
                                //                if (len < 40)
                                //                {
                                //                    sb.AppendLine((item[this.Column1]).ToString());
                                //                }
                                //                else
                                //                    sb.AppendLine((item[this.Column1]).ToString().Substring(0, 40) + "...");

                                //            }
                                //            else
                                //                sb.AppendLine(item[this.Column1].ToString()); 
                                //        }

                                //        //i = 11;
                                //        if (!string.IsNullOrEmpty(this.Column2) && item.Fields.ContainsField(this.Column2) && item[this.Column2] != null)
                                //        {
                                //            //sb.AppendLine(item.Fields[this.Column2].Type.ToString());
                                //            if (item.Fields[this.Column2].Type == SPFieldType.DateTime)
                                //            {
                                //                DateTime dt;
                                //                DateTime.TryParse(item[this.Column2].ToString(), out dt);

                                //                sb.AppendLine( dt.Date.ToString("yyyy-MM-dd"));
                                //            }
                                //            else
                                //            if (item.Fields[this.Column2].Type == SPFieldType.URL|| item.Fields[this.Column2].Type == SPFieldType.File)
                                //            {
                                //                SPFieldUrlValue fieldValue = new SPFieldUrlValue(item[this.Column2].ToString());

                                //                string linkTitle = fieldValue.Description;

                                //                string linkUrl = fieldValue.Url;

                                //                sb.AppendLine("<a href ='" + linkUrl + "' >" + linkTitle + "</a>");
                                //            }
                                //            else
                                //            if (item.Fields[this.Column2].Type == SPFieldType.Text)
                                //            {
                                //                int len = item.Fields[this.Column2].ToString().Length > 40 ? 40 : item.Fields[this.Column2].ToString().Length;
                                //                if (len < 40)
                                //                {
                                //                    sb.AppendLine((item[this.Column2]).ToString());
                                //                }
                                //                else
                                //                    sb.AppendLine( (item[this.Column2]).ToString().Substring(0, 40) + "...");

                                //            } 
                                //            else
                                //                sb.AppendLine((item[this.Column2].ToString()));
                                //         }

                                //        i = 12;
                                //        if (!string.IsNullOrEmpty(this.Column3) && item.Fields.ContainsField(this.Column3) && item[this.Column3] != null)
                                //        {
                                //            //sb.AppendLine(item.Fields[this.Column3].Type.ToString());
                                //            if (item.Fields[this.Column3].Type == SPFieldType.DateTime)
                                //            {
                                //                DateTime dt;
                                //                DateTime.TryParse(item[this.Column3].ToString(), out dt);

                                //                sb.AppendLine(" "+ dt.Date.ToString("yyyy-MM-dd"));
                                //            }
                                //            else
                                //            if (item.Fields[this.Column3].Type == SPFieldType.URL || item.Fields[this.Column3].Type == SPFieldType.File)
                                //            {
                                //                SPFieldUrlValue fieldValue = new SPFieldUrlValue(item[this.Column3].ToString());

                                //                string linkTitle = fieldValue.Description;

                                //                string linkUrl = fieldValue.Url;

                                //                sb.AppendLine("<a href ='" + linkUrl + "' >" + linkTitle + "</a>");
                                //            }
                                //            else
                                //            if (item.Fields[this.Column3].Type == SPFieldType.Text)
                                //            {
                                //                int len = item[this.Column3].ToString().Length > 40 ? 40 : item[this.Column3].ToString().Length;
                                //                if (len < 40)
                                //                {
                                //                    sb.AppendLine( (item[this.Column3]).ToString());
                                //                }
                                //                else
                                //                    sb.AppendLine( (item[this.Column3]).ToString().Substring(0, 40) + "...");

                                //            }
                                //            else
                                //                sb.AppendLine ((item[this.Column3]).ToString()); 
                                //        }
                                //    }
                                //    catch(Exception ex)
                                //    {
                                //        sb.Append(ex);
                                //    }
                                //    sb.AppendLine("</div>");                                     
                                //}
                                #endregion

                                sb.AppendLine("</div>");
                            }
                        }
                    } 
                    lblListView.InnerHtml = sb.ToString() + "<a href='"+this.MoreLink+"' style='float:right;'>More>></a>";
                    i = 6; 
                }
                else
                {
                    lblListView.InnerHtml = "Invalid Site Name";

                }
            }
            catch(Exception ex)
            {
                lblListView.InnerHtml = i +  sb.ToString() +"  Error: "+ ex.Message +ex.Source + ex.InnerException;
            }
        }

        public string setRowImagebyContentType(SPListItem spli)
        {

            //Use this for future
            /*
             if (Item.File != null) 
             {
                    SPUtility.MapToIcon(Web, Path.GetExtension(Item.File.Name), string.Empty, IconSize.Size16); 
             }
            */
            if (spli["File Type"]  != null && spli["File Type"].ToString().Length >  1)
                return "<img src = '/_layouts/images/ic" + spli["File Type"] + ".png' border = '0' />";
            else
            {
                switch(spli["ContentType"].ToString().ToLower())
                {
                    case "folder":
                        return "<img src = '/_layouts/images/folder.gif' border = '0'  height='16px'/>";
                    case "announcement":
                        return "<img src = '/_layouts/images/ANNOUNCE.GIF' border = '0' height='17px' />";
                    case "link":
                        return "<img src = '/_layouts/images/ApLinkPicker.gif' border = '0' height='17px'/>";
                }
            }
            
            return string.Empty;
        }
        public StringBuilder BuildtheContent(SPListItemCollection items, SPListTemplateType ltt)
        {
            StringBuilder sb = new StringBuilder();
            foreach (SPListItem item in items)
            {
                //sb.AppendLine("<br> ");
                sb.AppendLine("<div class='a4awpitempadding'>");

                int i = 10;
                try
                {
                    /** If the list of the type document library then add the icon */
                    string adsencodeurl = "";
                    sb.Append(setRowImagebyContentType(item));//item["ContentType"].ToString()));   //"ContentType" + item["ContentType"] + ".  " + item["File Type"] +".  ");

                    if (item["ContentType"].ToString().ToLower().Contains("folder") || ltt == SPListTemplateType.DocumentLibrary || ltt == SPListTemplateType.Links)
                    {
                        adsencodeurl = item["Encoded Absolute URL"].ToString();
                    }
                    else
                    if (ltt == SPListTemplateType.Announcements)
                    {
                        StringBuilder adsencodeurlSB = new StringBuilder();
                        adsencodeurl = item.ParentList.DefaultDisplayFormUrl + "?ID=" + item.ID;
                    }

                    ///Code for column1
                    if (!string.IsNullOrEmpty(this.Column1) && item.Fields.ContainsField(this.Column1) && item[this.Column1] != null)
                    { 
                        if (item.Fields[this.Column1].Type == SPFieldType.DateTime)
                        {
                            DateTime dt;
                            DateTime.TryParse(item[this.Column1].ToString(), out dt);
                            sb.AppendLine("<a href='"+ adsencodeurl + "'>"+dt.Date.ToString("yyyy-MM-dd")+"</a>");
                        } 
                        else
                        if (item.Fields[this.Column1].Type == SPFieldType.File)
                        {
                            SPFieldUrlValue fieldValue = new SPFieldUrlValue(item[this.Column1].ToString());

                            string linkTitle = fieldValue.Description;

                            string linkUrl = fieldValue.Url;

                            sb.AppendLine("<a href ='" + adsencodeurl + "' >" + linkTitle + "</a>");
                        }
                        else
                        if (item.Fields[this.Column1].Type == SPFieldType.URL)
                        {
                            SPFieldUrlValue fieldValue = new SPFieldUrlValue(item[this.Column1].ToString());

                            string linkTitle = fieldValue.Description;

                            string linkUrl = fieldValue.Url;

                            sb.AppendLine("<a href ='" + linkUrl + "' >" + linkTitle + "</a>");
                        }
                        else
                        if (item.Fields[this.Column1].Type == SPFieldType.Text)
                        {
                            int len = item.Fields[this.Column1].ToString().Length > 40 ? 40 : item[this.Column1].ToString().Length;
                            if (len < 40)
                            {
                                sb.AppendLine("<a href='" + adsencodeurl + "'>" + (item[this.Column1]).ToString() + "</a>");
                            }
                            else
                                sb.AppendLine("<a href='" + adsencodeurl + "'>" + (item[this.Column1]).ToString().Substring(0, 40) + "..." + "</a>");
                        }
                        else
                            sb.AppendLine(item["ContentType"]+"<a href='" + adsencodeurl + "'>" + item[this.Column1].ToString() + "..." + "</a>");
                    }
                    sb.AppendLine("<br />"); //Adding next line for column 2 and 3
                    //Code for column2
                    if (!string.IsNullOrEmpty(this.Column2) && item.Fields.ContainsField(this.Column2) && item[this.Column2] != null)
                    { 
                        if (item.Fields[this.Column2].Type == SPFieldType.DateTime)
                        {
                            DateTime dt;
                            DateTime.TryParse(item[this.Column2].ToString(), out dt);

                            sb.AppendLine(dt.Date.ToString("yyyy-MM-dd"));
                        }
                        else
                        if (item.Fields[this.Column2].Type == SPFieldType.URL || item.Fields[this.Column2].Type == SPFieldType.File)
                        {
                            SPFieldUrlValue fieldValue = new SPFieldUrlValue(item[this.Column2].ToString());

                            string linkTitle = fieldValue.Description;

                            string linkUrl = fieldValue.Url;

                            sb.AppendLine("<a href ='" + linkUrl + "' >" + linkTitle + "</a>");
                        }
                        else
                        if (item.Fields[this.Column2].Type == SPFieldType.Text)
                        {
                            int len = item.Fields[this.Column2].ToString().Length > 40 ? 40 : item.Fields[this.Column2].ToString().Length;
                            if (len < 40)
                            {
                                sb.AppendLine((item[this.Column2]).ToString());
                            }
                            else
                                sb.AppendLine((item[this.Column2]).ToString().Substring(0, 40) + "...");

                        }
                        else
                            sb.AppendLine((item[this.Column2].ToString()));
                    }

                    i = 12;
                    if (!string.IsNullOrEmpty(this.Column3) && item.Fields.ContainsField(this.Column3) && item[this.Column3] != null)
                    { 
                        if (item.Fields[this.Column3].Type == SPFieldType.DateTime)
                        {
                            DateTime dt;
                            DateTime.TryParse(item[this.Column3].ToString(), out dt);

                            sb.AppendLine(" " + dt.Date.ToString("yyyy-MM-dd"));
                        }
                        else
                        if (item.Fields[this.Column3].Type == SPFieldType.URL || item.Fields[this.Column3].Type == SPFieldType.File)
                        {
                            SPFieldUrlValue fieldValue = new SPFieldUrlValue(item[this.Column3].ToString());

                            string linkTitle = fieldValue.Description;

                            string linkUrl = fieldValue.Url;

                            sb.AppendLine("<a href ='" + linkUrl + "' >" + linkTitle + "</a>");
                        }
                        else
                        if (item.Fields[this.Column3].Type == SPFieldType.Text)
                        {
                            int len = item[this.Column3].ToString().Length > 40 ? 40 : item[this.Column3].ToString().Length;
                            if (len < 40)
                            {
                                sb.AppendLine((item[this.Column3]).ToString());
                            }
                            else
                                sb.AppendLine((item[this.Column3]).ToString().Substring(0, 40) + "...");

                        }
                        else
                            sb.AppendLine((item[this.Column3]).ToString());
                    }
                }
                catch (Exception ex)
                {
                    sb.Append(ex);
                }
                sb.AppendLine("</div>");
            }


            return sb;

        }

        public object GetColumnValue1(object val, SPFieldType type)
        {
            if (val == null)
                return string.Empty;

            if(type == SPFieldType.DateTime)
                return "type:"+type+((DateTime)val).Date;

            if (type == SPFieldType.Text)
                return "1. "+val.ToString().Substring(0, 40) + "...";

            else
                return "2. "+val;

        }
    }
}
