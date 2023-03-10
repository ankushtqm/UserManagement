using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint;

namespace ATA.Member.Util.Fuels
{
    public class FuelsLists
    {
        public static SPList GetAirportList(SPWeb fuelsRoot)
        {
            return GetListByWebRelatvieUrl(fuelsRoot, "Lists/Airports");
        }

        #region Root lists with multi-lookup. need to sort.
        public static SPList GetCommitteesAndConsortiaList(SPWeb fuelsRoot)
        {
            return GetListByWebRelatvieUrl(fuelsRoot, "Lists/Committee%20and%20Consortia");
            //Committees and Consortia  ATA.EventHandlers.MultipleFacilityGroupBasedSecurity
            // LookupMulti" DisplayName="Airport" 	ID="{00245616-976c-488a-b2a0-a42f6bd08b13}" Name="Airport_x0020_Code1" 
            // LookupMulti" DisplayName="Facility"    ID="{d344e7e7-3707-46d9-8fab-47072438c4bc}" Name="Facility"
        }

        public static SPList GetCompaniesList(SPWeb fuelsRoot)
        {
            return GetListByWebRelatvieUrl(fuelsRoot, "Lists/Airlines");
            //Companies  ATA.EventHandlers.MultipleFacilityGroupBasedSecurity  ATA.EventHandlers.CompanyListEventHandler
            //LookupMulti"   				ID="{d344e7e7-3707-46d9-8fab-47072438c4bc}" Name="Facility"  />
        }

        public static SPList GetFuelAlertsList(SPWeb fuelsRoot)
        {
            return GetListByWebRelatvieUrl(fuelsRoot, "Lists/Fuel%20Alerts");
            //Fuel Alerts (Lists/Fuel%20Alerts) : sharepoint workflow
            //LookupMulti" DisplayName="Airport Details ID="{189e8fa3-f9d2-479f-9304-aff0c6083436}" Name="Airport_x0020_Details" 
        }
        #endregion   

        #region Root lists with single Airport lookup
        public static SPList GetFacilityList(SPWeb fuelsRoot)
        {
            return GetListByWebRelatvieUrl(fuelsRoot, "Lists/Airport%20Summary");
            //Facility Quick Reference (/Lists/Airport%20Summary) ATA.EventHandlers.FacilityBasedSecurity. ATA.EventHandlers.FacilityListEventHandler
            //Lookup" DisplayName="Airport"  		ID="{00245616-976c-488a-b2a0-a42f6bd08b13}" Name="Airport_x0020_Code1" 
        }

        public static SPList GetAirportVolumesList(SPWeb fuelsRoot)
        {
            return GetListByWebRelatvieUrl(fuelsRoot, "Lists/Airport%20Volumes");
            //http://ata.fuelportal.airlines.org/Lists/Airport%20Volumes added airportlookup after FuelsInhancementV2
        }
        #endregion 

        #region Fuels farm subweb lists with single Facility lookup
        public static SPList GetCapital_SpecialProjectsList(SPWeb fuelsFarmSubWeb)
        {
            return GetListByWebRelatvieUrl(fuelsFarmSubWeb, "Lists/Capital_Special%20Projects");
            //Capital/Special Projects(fuelfarms/Lists/Capital_Special%20Projects) --ATA.EventHandlers.SingleFacilityGroupBasedSecurity 
        }

        public static SPList GetCrisisCommunicationsList(SPWeb fuelsFarmSubWeb)
        {
            return GetListByWebRelatvieUrl(fuelsFarmSubWeb, "Crisis%20Communications");
            //Crisis Communications (fuelfarms/Crisis%20Communications) --ATA.EventHandlers.SingleFacilityGroupBasedSecurity 
        }

        public static SPList GetDeliveryToAirplaneList(SPWeb fuelsFarmSubWeb)
        {
            return GetListByWebRelatvieUrl(fuelsFarmSubWeb, "Lists/Delivery%20to%20Airplane");
            //Delivery to Airplane (fuelfarms/Lists/Delivery%20to%20Airplane) --ATA.EventHandlers.SingleFacilityGroupBasedSecurity
        }

        public static SPList GetDeliveryToFacilityList(SPWeb fuelsFarmSubWeb)
        {
            return GetListByWebRelatvieUrl(fuelsFarmSubWeb, "Lists/Delivery%20To%20Facility");
            //Delivery To Facility (fuelfarms/Lists/Delivery%20To%20Facility) --ATA.EventHandlers.SingleFacilityGroupBasedSecurity
        }

        public static SPList GetFacilityDocumentsList(SPWeb fuelsFarmSubWeb)
        {
            return GetListByWebRelatvieUrl(fuelsFarmSubWeb, "Facility%20Documents");
            //Facility Documents (fuelfarms/Facility%20Documents) --ATA.EventHandlers.SingleFacilityGroupBasedSecurity
        }

        public static SPList GetOrganizationAndOperationsList(SPWeb fuelsFarmSubWeb)
        {
            return GetListByWebRelatvieUrl(fuelsFarmSubWeb, "Lists/Organization%20and%20Operations");
            //Organization and Operations (fuelfarms/Lists/Organization%20and%20Operations): --ATA.EventHandlers.SingleFacilityGroupBasedSecurity
        }

        public static SPList GetPhotoGalleryList(SPWeb fuelsFarmSubWeb)
        {
            return GetListByWebRelatvieUrl(fuelsFarmSubWeb, "Photo%20Gallery");
            //Photo Gallery (fuelfarms/Photo%20Gallery) --ATA.EventHandlers.SingleFacilityGroupBasedSecurity
        }

        public static SPList GetStorageList(SPWeb fuelsFarmSubWeb)
        {
            return GetListByWebRelatvieUrl(fuelsFarmSubWeb, "Lists/Storage");
            //Storage (fuelfarms/Lists/Storage)  ATA.EventHandlers.SingleFacilityGroupBasedSecurity
        } 
        #endregion

        #region Fuels farm subweb lists with single Airport lookup
        public static SPList GetSuppliersList(SPWeb fuelsFarmSubWeb)
        {
            return GetListByWebRelatvieUrl(fuelsFarmSubWeb, "Lists/Suppliers");
            //Suppliers (fuelfarms/Lists/Suppliers) //Lookup" DisplayName="Airport"   	ID="{00245616-976c-488a-b2a0-a42f6bd08b13}" 
        } 
        #endregion


        #region Private
        //        Ignored List:
        //Fuel Alerts (Ignore this list) in fuels farm
        //LookupMulti" DisplayName="Airport Code" ID="{00245616-976c-488a-b2a0-a42f6bd08b13}"  Name="Airport_x0020_Code1" 

        private static SPList GetListByWebRelatvieUrl(SPWeb web, string listUrl)
        {
            return web.GetList(SPUrlUtility.CombineUrl(web.Url, listUrl));
        }
        #endregion
    }
}
