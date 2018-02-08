export class SiteSourceViewModel{
    RecId: string;
    SiteName: string;
    SiteShortName: string;
    //Mandatory: boolean;
    ExtractionMode: string;
    SiteUrl: string;
    //ExcludeSI: boolean;
    //ExcludePI: boolean;
}

export class Country{
    CountryName: string;
    SiteId: string;
    IsMandatory: boolean;
    SiteUrl: string;
    SearchAppliesTo: number;
    RecId: string;
    Name: string;
}

export class CountryViewModel{
    CountryName: string;
    SiteName: string;
    SiteId: string;
    SiteUrl: string;
    RecId: string;
}

export class SponsorProtocol{
    SponsorProtocolNumber: string;
    SiteId: string;
    IsMandatory: boolean;
    SiteUrl: string;
    SearchAppliesTo: number;
    RecId: string;
    Name: string;
}

 export class DefaultSite{
    RecId : string;
    Name: string;
    SiteId : string;
    OrderNo : number;
    IsMandatory : boolean;
    SearchAppliesTo: number;
    //ExcludeSI : boolean;
}

// export class DownloadDataFilesViewModel{
//         FileName: string;
//         FullPath: string;
//         SiteName: string;
//         DownloadedOn: Date;
//         FileSize: number;
//         FileType: string;
// }