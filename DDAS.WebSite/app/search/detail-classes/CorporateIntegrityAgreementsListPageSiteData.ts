import {SiteDataItemBase, SiteData} from '../search.classes';

export class CorporateIntegrityAgreementsListPageSiteData extends SiteData
{
      DebarredPersons : DebarredPerson[];
}

export class DebarredPerson extends SiteDataItemBase
  {
    NameOfPerson:string;
    EffectiveDate:string;
    EndOfTermOfDebarment:string;
    FrDateText:string;
    VolumePage:string;
    DocumentLink:string;
    DocumentName:string;
  }  



