import {SiteDataItemBase, SiteData} from '../search.classes';

export class SpeciallyDesignedNationalsPageSiteData extends SiteData
{
      SDNListSiteData : DebarredPerson[];
}

export class DebarredPerson extends SiteDataItemBase
  {
    Name:string;
    PageNumber:number;
    RecordNumber:number;
    WordsMatched:string;
  }  



