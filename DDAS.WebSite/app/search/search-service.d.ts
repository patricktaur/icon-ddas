import { Http } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { StudyNumbers, SearchSummary, SearchResultSaveData, SitesIncludedInSearch } from './search.classes';
import { ConfigService } from '../shared/utils/config.service';
export declare class SearchService {
    private http;
    private configService;
    _baseUrl: string;
    _controller: string;
    constructor(http: Http, configService: ConfigService);
    getStudyNumbers(): Observable<StudyNumbers[]>;
    getSearchSummary(NameToSearch: string): Observable<SearchSummary>;
    getSearchSummaryDetails(NameToSearch: string, RecId: string, SiteEnum: number): Observable<any>;
    saveCheckedResults(markedResults: SearchResultSaveData): Observable<any>;
    saveSiteDetails(siteDetails: SitesIncludedInSearch, complianceFormId: string): Observable<any>;
    private extractData(res);
    private handleError(error);
}
