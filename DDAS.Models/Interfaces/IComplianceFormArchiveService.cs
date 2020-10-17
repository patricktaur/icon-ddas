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

 
       
        string GetUserFullName(string UserName);

        string ArchiveComplianceFormsWithSearchDaysGreaterThan(int days);

    }
}
