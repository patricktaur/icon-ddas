export class SiteSourceViewModel{
    RecId: string;
    SiteName: string;
    SiteShortName: string;
    Mandatory: boolean;
    ExtractionMode: string;
    SiteUrl: string;
    ExcludeSI: boolean;
    ExcludePI: boolean;
}

export class Country{
    Name: string;
    SiteId: string;
}

export class CountryViewModel{
    Name: string;
    SiteName: string;
    SiteId: string;
}


export class SponsorProtocol{
    SponsorProtocolNumber: string;
    SiteId: string;
}