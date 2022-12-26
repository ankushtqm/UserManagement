//#region namespaces

//using System;
//using System.Collections.Generic;
//using System.Text;
//using Microsoft.SharePoint;
//using Microsoft.SharePoint.Administration;

//#endregion

//namespace ATA.Authentication
//{
//    class FeatureReceiver : SPFeatureReceiver
//    {
//        #region FeatureActivated

//        public override void FeatureActivated(SPFeatureReceiverProperties properties)
//        {
//            //add an entry for site list service url
//            //derive from the surrent site?
//            ProcessWebConfigChange(properties, false);
//        }

//        #endregion

//        #region FeatureDeactivating

//        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
//        {
//            //remove our entry in the web.config
//            ProcessWebConfigChange(properties, true);
//        }

//        #endregion

//        #region ProcessWebConfigChange

//        private void ProcessWebConfigChange(SPFeatureReceiverProperties properties, bool removeModification)
//        {
//            SPWebApplication app = null;
//            SPSite site = properties.Feature.Parent as SPSite;

//            if (site == null)
//                return;  //this should not happen


//            app = site.WebApplication;
//            if (app != null)
//            {
//                string ATAMembershipConnectionName = "ATA_MembershipConnection";

//                //add connection string name for data objects
//                SPWebConfigModification modification = new SPWebConfigModification("ATA.MembershipAppSettings", "configuration/appSettings");
//                modification.Owner = "ATA.Authentication";
//                modification.Sequence = 0;
//                modification.Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode;
//                modification.Value = string.Format("<add key=\"DataObjectConnectionStringName\" value=\"{0}\" />", ATAMembershipConnectionName);

//                if (removeModification)
//                    app.WebConfigModifications.Remove(modification);
//                else
//                    app.WebConfigModifications.Add(modification);

//                //add connection string
//                modification = new SPWebConfigModification("ATA.MembershipConnectionStrings", "configuration/connectionStrings");
//                modification.Owner = "ATA.Authentication";
//                modification.Sequence = 1;
//                modification.Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode;
//                modification.Value = string.Format("<add name=\"{0}\" connectionString=\"{1}\" />", ATAMembershipConnectionName, "Server={Server};Database={Database};Uid={userId};Pwd={Password}");

//                if (removeModification)
//                    app.WebConfigModifications.Remove(modification);
//                else
//                    app.WebConfigModifications.Add(modification);



//                //add membership sections
//                StringBuilder sb = new StringBuilder();
//                sb.Append("<membership defaultProvider=\"ATAMembershipProvider\">");
//                sb.Append("<providers>");
//                sb.Append("<add ");
//                sb.Append("name=\"ATAMembershipProvider\" ");
//                sb.Append("type=\"ATA.Authentication.Providers.ATAMembershipProvider, ATA.Authentication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=65b988712bd31de0\" ");
//                sb.Append("enablePasswordRetrieval=\"true\" ");
//                sb.Append("enablePasswordReset=\"true\" ");
//                sb.Append("applicationName=\"/\" ");
//                sb.Append("maxInvalidPasswordAttempts=\"5\" ");
//                sb.Append("minRequiredPasswordLength=\"7\" ");
//                sb.Append("minRequiredNonalphanumericCharacters=\"1\" ");
//                sb.Append("passwordFormat=\"Clear\" ");
//                sb.Append("passwordStrengthRegularExpression=\"\" ");
//                sb.Append("passwordAttemptWindow=\"10\" ");
//                sb.Append("requiresQuestionAndAnswer=\"false\" ");
//                sb.Append("requiresUniqueEmail=\"true\" ");
//                sb.AppendFormat("connectionStringName=\"{0}\" ", ATAMembershipConnectionName);
//                sb.Append(" />");
//                sb.Append("</providers>");
//                sb.Append("</membership>");
//                sb.Append("<!-- role provider -->");
//                sb.Append("<roleManager enabled=\"true\" defaultProvider=\"ATARoleProvider\">");
//                sb.Append("<providers>");
//                sb.Append("<add ");
//                sb.Append("applicationName=\"/\" ");
//                sb.Append("name=\"ATARoleProvider\" ");
//                sb.Append("type=\"ATA.Authentication.Providers.ATARoleProvider, ATA.Authentication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=65b988712bd31de0\" ");
//                sb.AppendFormat("connectionStringName=\"{0}\" ", ATAMembershipConnectionName);
//                sb.Append(" />");
//                sb.Append("</providers>");
//                sb.Append("</roleManager>");

//                SPWebConfigModification membershipBlock = new SPWebConfigModification("ATA.AuthenticationMembership", "configuration/system.web");
//                membershipBlock.Owner = "ATA.Authentication";
//                membershipBlock.Sequence = 2;
//                membershipBlock.Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode;
//                membershipBlock.Value = sb.ToString();

//                if (removeModification)
//                    app.WebConfigModifications.Remove(membershipBlock);
//                else
//                    app.WebConfigModifications.Add(membershipBlock);

//                SPFarm.Local.Services.GetValue<SPWebService>(app.Parent.Id).ApplyWebConfigModifications();
//            }
//            else
//                throw new ApplicationException("Could not locate a web application");
//        }

//        #endregion

//        #region FeatureInstalled (Not Implemented)

//        public override void FeatureInstalled(SPFeatureReceiverProperties properties)
//        {
//            return;
//        }

//        #endregion

//        #region FeatureUninstalling (Not Implemented)

//        public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
//        {
//            return;
//        }

//        #endregion
//    }
//}
