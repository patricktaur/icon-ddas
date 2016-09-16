import {Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription }       from 'rxjs/Subscription';
import {SlimLoadingBarService} from 'ng2-slim-loading-bar/ng2-slim-loading-bar';
//import { NotificationService } from '../shared/utils/notification.service';

import { searchRoutes }        from './search.routing';


import {SearchService} from './search-service';
import {IStudyNumber, SearchResult, SearchQuery, SearchQueryAtSite, ResultAtSite, SearhQuerySite} from './search-interfaces-classes';

@Component({
    moduleId: module.id,
    //selector: 'searchPage',
    templateUrl: 'search.component.html'


})
export class SearchComponent
    implements OnInit, OnDestroy {
    private sub: Subscription;
    private StudyNumbers: IStudyNumber[];
    private searchResult: SearchResult;
    private searchQuery: SearchQuery;

    constructor(private service: SearchService,
        private slimLoader: SlimLoadingBarService
    ) { }

    ngOnInit() {
        this.searchQuery = new SearchQuery;
        this.searchResult = new SearchResult;
        this.LoadStudyNumbers();
        this.LoadNewSearchQuery();
    }

    LoadNewSearchQuery() {
        this.slimLoader.start();
        this.service.getSearchQuery()
            .subscribe((item: SearchQuery) => {
                this.searchQuery = item;
                this.slimLoader.complete();
            },
            error => {
                //this.slimLoader.complete();
                //this.notificationService.printErrorMessage('Failed to load users. ' + error);
            });
    }

    LoadStudyNumbers() {
        this.slimLoader.start();
        this.service.getStudyNumbers()

            .subscribe((items: IStudyNumber[]) => {
                this.StudyNumbers = items;
                this.slimLoader.complete();
            },
            error => {
                //this.slimLoader.complete();
                //this.notificationService.printErrorMessage('Failed to load users. ' + error);
            });
    }

    ClearAllSearchResults(){
        this.searchQuery.SearchSites.forEach(site => {
            this.ClearSearchResultsAtSite(site);
        });
    }
    ClearSearchResultsAtSite(site: SearhQuerySite){
        site.Processing = false;
        site.Results = [];
    }
    
    SearchResultsAllSites() {

        this.searchQuery.SearchSites.forEach(site => {
            if (site.Selected){
                this.SearchResultAtSite(site.SiteEnum);
            }
         });
    }

    SearchResultAtSite(siteEnum: number) {
        this.slimLoader.start();
        var query = new SearchQueryAtSite();
        query.NameToSearch = this.searchQuery.NameToSearch
        query.SiteEnum = siteEnum;

        var site = this.searchQuery.SearchSites.find((obj: SearhQuerySite) => obj.SiteEnum == siteEnum)
        if (site == undefined) {
            throw new TypeError("site object not found");
        }
        site.Results = [];
        site.Processing = true;
        this.service.SearchResultsAtSite(query)
            .subscribe((item: ResultAtSite) => {

                /*
                if (item.SiteEnum != siteEnum) {
                    throw new TypeError("Incorrect SiteEnum received");
                }              
                */


                item.Results.forEach(element => {
                    site.Results.push(element);
                });
                site.Processing = false;
                this.slimLoader.complete();
            },
            error => {
                //this.slimLoader.complete();
                //this.notificationService.printErrorMessage('Failed to load users. ' + error);
            });

    }
   

    get diagnostic() { return JSON.stringify(this.searchQuery); }

    ngOnDestroy() {
        //this.sub.unsubscribe();
    }

}

