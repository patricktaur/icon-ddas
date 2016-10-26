import {SiteDataItemBase, SiteData} from '../search.classes';

export class PHSPageSiteData extends SiteData
{
      PHSAdministrativeSiteData : DebarredPerson[];
}

export class DebarredPerson extends SiteDataItemBase
  {
    LastName:string;
    FirstName:string;
    MiddleName:string;
    DebarmentUntil:string;
    NoPHSAdvisoryUntil:string;
    CertificationOfWorkUntil:string;
    SupervisionUntil:string;
    RetractionOfArticle:string;
    CorrectionOfArticle:string;
    Memo:string;
  }  



