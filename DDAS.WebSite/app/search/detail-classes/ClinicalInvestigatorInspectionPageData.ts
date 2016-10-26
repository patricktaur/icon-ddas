import {SiteDataItemBase, SiteData} from '../search.classes';

export class ClinicalInvestigatorInspectionPageData extends SiteData
{
      ClinicalInvestigatorInspectionList : DebarredPerson[];
}

export class DebarredPerson extends SiteDataItemBase
  {
    
    IdNumber:string;
    Name:string;
    Location:string;
    Address:string;
    City:string;
    State:string;
    Country:string;
    Zipcode:string;
    InspectionDate:string;
    ClassificationType:string;
    ClassificationCode:string;
    DeficienyCode:string;
    
  }  


