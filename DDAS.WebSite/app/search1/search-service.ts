import { Injectable } from '@angular/core';
import { Http, Response, Headers } from '@angular/http';
//Grab everything with import 'rxjs/Rx';
import { Observable } from 'rxjs/Observable';
import {Observer} from 'rxjs/Observer';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';

import {  Pagination, PaginatedResult } from '../shared/interfaces';
import { ISearchHistory, IStudyNumber,  SearchResult, SearchQuery, SearchQueryAtSite, ResultAtSite} from './search-interfaces-classes';


import { ConfigService } from '../shared/utils/config.service';

@Injectable()
export class SearchService {

    _baseUrl: string = '';

    constructor(private http: Http,
       
        private configService: ConfigService) {
        this._baseUrl = configService.getApiURI();
    }

   
    getSearchHistory(): Observable<ISearchHistory[]> {
        
        
        return this.http.get(this._baseUrl + 'search/SearchHistory')
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

     getStudyNumbers(): Observable<IStudyNumber[]> {
        return this.http.get(this._baseUrl + 'search/StudyNumbers')
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }
 
    getSearchQuery(): Observable<SearchQuery> {
        return this.http.get(this._baseUrl + 'search/getNewSearchQuery')
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }
    
   
    SearchResultsAllSites(searchquery: SearchQuery): Observable<SearchResult>
     {
 
        let headers = new Headers();
        headers.append('Content-Type', 'application/json');

        return this.http.post(this._baseUrl + 'search/SearchResult/', JSON.stringify(searchquery), {
            headers: headers
        })
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }
    
    SearchResultsAtSite(searchquery: SearchQueryAtSite): Observable<ResultAtSite>
     {
 
        let headers = new Headers();
        headers.append('Content-Type', 'application/json');

        return this.http.post(this._baseUrl + 'search/SearchResultAtSite/', JSON.stringify(searchquery), {
            headers: headers
        })
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }
    private handleError(error: any) {
        var applicationError = error.headers.get('Application-Error');
        var serverError = error.json();
        var modelStateErrors: string = '';
           
        if (!serverError.type) {
            console.log(serverError);
            for (var key in serverError) {
                if (serverError[key])
                    modelStateErrors += serverError[key] + '\n';
            }
        }

        modelStateErrors = modelStateErrors = '' ? null : modelStateErrors;

        return Observable.throw(applicationError || modelStateErrors || 'Server error');
    }
}