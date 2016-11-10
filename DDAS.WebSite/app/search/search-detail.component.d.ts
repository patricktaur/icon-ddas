import { Location } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { SiteData, MatchedRecordsPerSite } from './search.classes';
import { SearchService } from './search-service';
export declare class SearchDetailComponent {
    private service;
    private _location;
    private route;
    private displayTitle;
    private displayName;
    private detailItems;
    private _isChecked;
    _SiteData: SiteData;
    private siteDetails;
    matchedRecords: MatchedRecordsPerSite;
    constructor(service: SearchService, _location: Location, route: ActivatedRoute);
    ngOnInit(): void;
    LoadSiteResultDetails(): void;
    markApproved(): void;
    markRejected(): void;
    clearSelected(): void;
    checkAll(): void;
    clearAllSelection(): void;
    saveSiteDetails(): void;
    saveMarkedResults(): void;
    goBack(): void;
    readonly diagnostic: string;
}
