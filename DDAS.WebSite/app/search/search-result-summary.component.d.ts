import { Router, ActivatedRoute } from '@angular/router';
import { SiteInfo, SearchSummaryItem, SearchSummary } from './search.classes';
import { Location } from '@angular/common';
import { SearchService } from './search-service';
export declare class SearchResultSummaryComponent {
    private service;
    private route;
    private _location;
    private router;
    NameToSearch: string;
    private SearchName;
    SearchSummary: SearchSummary;
    SearchSummaryItems: SearchSummaryItem[];
    SiteName: string;
    SiteEnum: number;
    w3site: string;
    processing: boolean;
    constructor(service: SearchService, route: ActivatedRoute, _location: Location, router: Router);
    ngOnInit(): void;
    LoadSearchSummary(): void;
    gotoSearch(): void;
    onSelect(summary: SiteInfo): void;
    onSelectedSite(summary: SearchSummaryItem): void;
    readonly diagnostic: string;
}
