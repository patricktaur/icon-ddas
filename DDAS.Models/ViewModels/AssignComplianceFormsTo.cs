﻿using DDAS.Models.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.ViewModels
{
    public class AssignComplianceFormsTo
    {
        public string AssignedTo { get; set; }
        public List<PrincipalInvestigator> PrincipalInvestigators { get; set; }
    }
}
