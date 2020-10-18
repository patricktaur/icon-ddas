using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using DDAS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;

namespace DDAS.Models.Interfaces
{
    public interface IComplianceFormArchiveService
    {

 
       
        

        string ArchiveComplianceFormsWithSearchDaysGreaterThan(int days);

        //ComplianceForm GetComplianceForm(Guid ComplianceFormId);
        //List<PrincipalInvestigator> getAllPrincipalInvestigators();
        //List<PrincipalInvestigator> getPrincipalInvestigators(string AssignedTo, bool Active);
        //List<PrincipalInvestigator> getPrincipalInvestigators(string AssignedTo, bool Active = true, bool ReviewCompleted = true);
        //List<PrincipalInvestigator> getPrincipalInvestigators(string AssignedTo, ReviewStatusEnum ReviewStatus);
        List<PrincipalInvestigatorArchive> getPrincipalInvestigators(List<ComplianceFormArchive> Forms);



        
        List<PrincipalInvestigatorArchive> GetComplianceFormsFromFiltersWithReviewDates(
            ComplianceFormArchiveFilter CompFormFilter);


        //InvestigatorSearched getInvestigatorSiteSummary(string compFormId, int InvestigatorId);
        string GetUserFullName(string UserName);

    }
}
