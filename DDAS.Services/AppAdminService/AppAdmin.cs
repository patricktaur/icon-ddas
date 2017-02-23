using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Services.AppAdminService
{
    public class AppAdmin : IAppAdmin
    {
        private IUnitOfWork _UOW;
        public AppAdmin(IUnitOfWork UOW)
        {
            _UOW = UOW;
        }

        public List<DataExtractionHistory> GetDataExtractionHistory()
        {
            var FDADebarSiteData = _UOW.FDADebarPageRepository.GetAll().
                OrderByDescending(x => x.CreatedOn).
                First();

            var ListOfExtractionHistory = new List<DataExtractionHistory>();
            var ExtractionHistory1 = new DataExtractionHistory();

            ExtractionHistory1.SiteNumber = 1;
            ExtractionHistory1.Enum = SiteEnum.FDADebarPage;
            ExtractionHistory1.ExtractionDate = FDADebarSiteData.CreatedOn;
            if(FDADebarSiteData.DataExtractionErrorMessage != null)
            {
                ExtractionHistory1.ErrorDescription = 
                    FDADebarSiteData.DataExtractionErrorMessage;
            }
            else if(!FDADebarSiteData.DataExtractionRequired)
            {
                ExtractionHistory1.ExtractionMessage = "Source Date is not updated";
            }
            else
                ExtractionHistory1.ExtractionMessage = "Data extracted successfully";

            ExtractionHistory1.SiteLastUpdatedOn = FDADebarSiteData.SiteLastUpdatedOn;

            ListOfExtractionHistory.Add(ExtractionHistory1);

            var ExtractionHistory2 = new DataExtractionHistory();

            var ClinicalInvestigatorSiteData =
                _UOW.ClinicalInvestigatorInspectionListRepository.GetAll().
                OrderByDescending(x => x.CreatedOn).
                First();

            ExtractionHistory2.SiteNumber = 2;
            ExtractionHistory2.Enum = SiteEnum.ClinicalInvestigatorInspectionPage;
            ExtractionHistory2.ExtractionDate = ClinicalInvestigatorSiteData.CreatedOn;
            if (ClinicalInvestigatorSiteData.DataExtractionErrorMessage != null)
            {
                ExtractionHistory2.ErrorDescription =
                    ClinicalInvestigatorSiteData.DataExtractionErrorMessage;
            }
            else if(!ClinicalInvestigatorSiteData.DataExtractionRequired)
                ExtractionHistory2.ExtractionMessage = "Source Date is not updated";
            else
                ExtractionHistory2.ExtractionMessage = "Data extracted successfully";

            ExtractionHistory2.SiteLastUpdatedOn =
                ClinicalInvestigatorSiteData.SiteLastUpdatedOn;
                
            ListOfExtractionHistory.Add(ExtractionHistory2);

            var ExtractionHistory3 = new DataExtractionHistory();

            var ERRSiteData = _UOW.ERRProposalToDebarRepository.GetAll().
                OrderByDescending(x => x.CreatedOn).
                First();

            ExtractionHistory3.SiteNumber = 3;
            ExtractionHistory3.Enum = SiteEnum.ERRProposalToDebarPage;
            ExtractionHistory3.ExtractionDate = ERRSiteData.CreatedOn;
            if (ERRSiteData.DataExtractionErrorMessage != null)
            {
                ExtractionHistory3.ErrorDescription =
                    ERRSiteData.DataExtractionErrorMessage;
            }
            else if (!ERRSiteData.DataExtractionRequired)
                ExtractionHistory3.ExtractionMessage = "Source Date is not updated";
            else
                ExtractionHistory3.ExtractionMessage = "Data extracted successfully";

            ExtractionHistory3.SiteLastUpdatedOn =
                ERRSiteData.SiteLastUpdatedOn;

            ListOfExtractionHistory.Add(ExtractionHistory3);

            var ExtractionHistory4 = new DataExtractionHistory();

            var AdequateAssuraceSiteData = _UOW.AdequateAssuranceListRepository.GetAll().
                OrderByDescending(x => x.CreatedOn).
                First();

            ExtractionHistory4.SiteNumber = 4;
            ExtractionHistory4.Enum = SiteEnum.AdequateAssuranceListPage;
            ExtractionHistory4.ExtractionDate = AdequateAssuraceSiteData.CreatedOn;
            if (AdequateAssuraceSiteData.DataExtractionErrorMessage != null)
            {
                ExtractionHistory4.ErrorDescription =
                    AdequateAssuraceSiteData.DataExtractionErrorMessage;
            }
            else if (!AdequateAssuraceSiteData.DataExtractionRequired)
                ExtractionHistory4.ExtractionMessage = "Source Date is not updated";
            else
                ExtractionHistory4.ExtractionMessage = "Data extracted successfully";

            ExtractionHistory4.SiteLastUpdatedOn =
                AdequateAssuraceSiteData.SiteLastUpdatedOn;

            ListOfExtractionHistory.Add(ExtractionHistory4);

            var ExtractionHistory5 = new DataExtractionHistory();

            var PHSSiteData = _UOW.PHSAdministrativeActionListingRepository.GetAll().
                OrderByDescending(x => x.CreatedOn).
                First();

            ExtractionHistory5.SiteNumber = 5;
            ExtractionHistory5.Enum = SiteEnum.PHSAdministrativeActionListingPage;
            ExtractionHistory5.ExtractionDate = PHSSiteData.CreatedOn;
            if (PHSSiteData.DataExtractionErrorMessage != null)
            {
                ExtractionHistory5.ErrorDescription =
                    PHSSiteData.DataExtractionErrorMessage;
            }
            else if (!PHSSiteData.DataExtractionRequired)
                ExtractionHistory5.ExtractionMessage = "Source Date is not updated";
            else
                ExtractionHistory5.ExtractionMessage = "Data extracted successfully";

            ExtractionHistory5.SiteLastUpdatedOn = PHSSiteData.SiteLastUpdatedOn;

            ListOfExtractionHistory.Add(ExtractionHistory5);

            var ExtractionHistory6 = new DataExtractionHistory();

            var CBERSiteData = _UOW.CBERClinicalInvestigatorRepository.GetAll().
                OrderByDescending(x => x.CreatedOn).
                First();

            ExtractionHistory6.SiteNumber = 6;
            ExtractionHistory6.Enum = SiteEnum.CBERClinicalInvestigatorInspectionPage;
            ExtractionHistory6.ExtractionDate = CBERSiteData.CreatedOn;                
            if (CBERSiteData.DataExtractionErrorMessage != null)
            {
                ExtractionHistory6.ErrorDescription =
                    CBERSiteData.DataExtractionErrorMessage;
            }
            else if (!CBERSiteData.DataExtractionRequired)
                ExtractionHistory6.ExtractionMessage = "Source Date is not updated";
            else
                ExtractionHistory6.ExtractionMessage = "Data extracted successfully";

            ExtractionHistory6.SiteLastUpdatedOn = CBERSiteData.SiteLastUpdatedOn;

            ListOfExtractionHistory.Add(ExtractionHistory6);

            var ExtractionHistory7 = new DataExtractionHistory();

            var ExclusionSiteData = _UOW.ExclusionDatabaseSearchRepository.GetAll().
                OrderByDescending(x => x.CreatedOn).
                First();

            ExtractionHistory7.SiteNumber = 7;
            ExtractionHistory7.Enum = SiteEnum.ExclusionDatabaseSearchPage;
            ExtractionHistory7.ExtractionDate = ExclusionSiteData.CreatedOn;
            if (ExclusionSiteData.DataExtractionErrorMessage != null)
            {
                ExtractionHistory7.ErrorDescription =
                    ExclusionSiteData.DataExtractionErrorMessage;
            }
            else if (!ExclusionSiteData.DataExtractionRequired)
                ExtractionHistory7.ExtractionMessage = "Source Date is not updated";
            else
                ExtractionHistory7.ExtractionMessage = "Data extracted successfully";

            ExtractionHistory7.SiteLastUpdatedOn =
                ExclusionSiteData.SiteLastUpdatedOn;

            ListOfExtractionHistory.Add(ExtractionHistory7);

            var ExtractionHistory8 = new DataExtractionHistory();

            var CIASiteData = _UOW.CorporateIntegrityAgreementRepository.GetAll().
                OrderByDescending(x => x.CreatedOn).
                First();

            ExtractionHistory8.SiteNumber = 8;
            ExtractionHistory8.Enum = SiteEnum.CorporateIntegrityAgreementsListPage;
            ExtractionHistory8.ExtractionDate = CIASiteData.CreatedOn;
            if (CIASiteData.DataExtractionErrorMessage != null)
            {
                ExtractionHistory8.ErrorDescription =
                    CIASiteData.DataExtractionErrorMessage;
            }
            else if (!CIASiteData.DataExtractionRequired)
                ExtractionHistory8.ExtractionMessage = "Source Date is not updated";
            else
                ExtractionHistory8.ExtractionMessage = "Data extracted successfully";

            ExtractionHistory8.SiteLastUpdatedOn = CIASiteData.SiteLastUpdatedOn;

            ListOfExtractionHistory.Add(ExtractionHistory8);

            var ExtractionHistory9 = new DataExtractionHistory();

            var SDNSiteData = _UOW.SpeciallyDesignatedNationalsRepository.GetAll().
                OrderByDescending(x => x.CreatedOn).
                First();

            ExtractionHistory9.SiteNumber = 9;
            ExtractionHistory9.Enum = SiteEnum.SpeciallyDesignedNationalsListPage;
            ExtractionHistory9.ExtractionDate = SDNSiteData.CreatedOn;
            if (SDNSiteData.DataExtractionErrorMessage != null)
            {
                ExtractionHistory9.ErrorDescription =
                    SDNSiteData.DataExtractionErrorMessage;
            }
            else if (!SDNSiteData.DataExtractionRequired)
                ExtractionHistory9.ExtractionMessage = "Source Date is not updated";
            else
                ExtractionHistory9.ExtractionMessage = "Data extracted successfully";

            ExtractionHistory9.SiteLastUpdatedOn =
                SDNSiteData.SiteLastUpdatedOn;

            ListOfExtractionHistory.Add(ExtractionHistory9);

            return ListOfExtractionHistory;
        }
    }
}
