using System;
using System.Collections.Generic;
using System.Text;

namespace ATA.Member.Util.Fuels
{
    public static class FuelsFieldNames
    {
        
        public const string AirportLookup = "Airport_x0020_Code1";//ID="{00245616-976c-488a-b2a0-a42f6bd08b13}" 
 
        //public const string AirportLookupForDocProperty = "Airport Code1"; 
        //The Airport code lookup site column created by ATA or susquetech has space in the name. Document lib and list treat the name with space differently 
        //The property name in list is field's internal name but in document library is "Airport Code1"


        //Create ATA_FL_AirportLinked new field in each list because Airport code lookup site column created by ATA or susquetech has space in the name. Document lib and list treat the name with space differently 
        //We need a conistent way to set the data. We didn't create a site column because there is already a airport site column.
        public const string AirportLinked = "ATA_FL_AirportLinked";
        public const string AirportRegionLinked = "ATA_FL_AirportRegionLinked";
        public const string AirportStateLinked = "ATA_FL_AirportStateLinked"; 


        public const string FacilityLookup = "Facility";//ID="{d344e7e7-3707-46d9-8fab-47072438c4bc}" 


        public static class AirportList
        {
            public const string Region = "Region";
            //We have messy field internal names because Susquetech didn't use feature or code to create fields
            public const string State = "State_x002f_Prov";
            public const string AirportCode = "Airport_x0020_Code1";
        }

        public static class FacilityList
        {
            public const string Facility = "Facility"; 
        }

        public static class CompanyList
        {
            public const string CompanyName = "Title";
        }

        public static class CommitteeAndConsortiaList
        {
            public const string Members = "Member_x0028_s_x0029_";
        } 
    }
}
