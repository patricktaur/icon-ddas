import { Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
//Grab everything with import 'rxjs/Rx';
import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';

import { Pagination, PaginatedResult } from '../shared/interfaces';
import {
    SiteInfo, StudyNumbers, SearchSummaryItem, SearchSummary, NameSearch,
    SearchResultSaveData, 
    //Site, 
    ComplianceFormA,
    SiteSource,
    CompFormFilter,
    PrincipalInvestigatorDetails
} from './search.classes';

//import {FDADebarPageSiteData} from './detail-classes/FDADebarPageSiteData';
//import {NameSearchSiteDetails5} from './site-result-detail.class-5';


import { ConfigService } from '../shared/utils/config.service';
import { AuthService } from '../auth/auth.service';

@Injectable()
export class SearchService {
    _baseUrl: string = '';
    //_controller: string = 'search/'; 
    _controller: string = '';
    _options: RequestOptions;

    constructor(private http: Http,
        private configService: ConfigService,
        private authService: AuthService
        ) {
        this._baseUrl = configService.getApiURI() + this._controller;
          let headers = new Headers();
            headers.append('Content-Type', 'application/json');
            headers.append("Authorization","Bearer " + this.authService.token);
            this._options = new RequestOptions({headers: headers});
        }

    getStudyNumbers(): Observable<StudyNumbers[]> {
       
        return this.http.get(this._baseUrl + 'StudyNumbers')
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);

    }



  

    CloseComplianceForm(RecId: string) {
         return this.http.put(this._baseUrl + 'search/CloseComplianceForm?ComplianceFormId=' + RecId, null, this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

    OpenComplianceForm(RecId: string) {
        return this.http.put(this._baseUrl + 'search/OpenComplianceForm?ComplianceFormId=' + RecId, null, this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

    deleteComplianceForm(CompFormId: string) {

        let headers = new Headers();
        headers.append('Content-Type', 'application/json');

        return this.http.get(this._baseUrl + 'search/DeleteComplianceForm/?ComplianceFormId=' + CompFormId, this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

    //Patrick 27Nov2016:--------------------
    
      getPrincipalInvestigators() {

        // let headers = new Headers();
        // headers.append('Content-Type', 'application/json');
        // headers.append("Authorization","Bearer " + this.authService.token);
        // let options = new RequestOptions({headers: headers});
        
 
        
    return this.http.get(this._baseUrl + 'search/GetPrincipalInvestigators', this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

    //Pradeep 5Jan2017
    getPrincipalInvestigatorsByFilters(Filters: CompFormFilter): Observable<PrincipalInvestigatorDetails[]> {
       let Filter1 = JSON.stringify(Filters);
      
        return this.http.post(this._baseUrl + 'search/ComplianceFormFilters', Filter1,  this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);

    }

    getAllUsers()
    {
        
        return this.http.get(this._baseUrl + 'Account/GetUsers', this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

    getMyReviewPendingPrincipalInvestigators() {
         
        return this.http.get(this._baseUrl + 'search/GetMyReviewPendingPrincipalInvestigators', this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

    getMyReviewCompletedPrincipalInvestigators() {
         
        return this.http.get(this._baseUrl + 'search/GetMyReviewCompletedPrincipalInvestigators', this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

    
    
    getComplianceForm(formId: string): Observable<ComplianceFormA> {
        
        return this.http.get(this._baseUrl + 'search/GetComplianceFormA/?formId=' + formId, this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

    SaveAssignedTo(AssignedTo: string, Active: boolean, ComplianceFormId: string): Observable<boolean> 
    {
        return this.http.get(this._baseUrl + 'search/SaveAssignedToData?' + 
        'AssignedTo=' + AssignedTo +'&Active=' + Active +'&ComplianceFormId=' + ComplianceFormId,
        this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

    saveComplianceForm(form: ComplianceFormA): Observable<ComplianceFormA> {
        let body = JSON.stringify(form);
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });

        return this.http.post(this._baseUrl + 'search/SaveComplianceForm', body, this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);

    }

    scanSaveComplianceForm(form: ComplianceFormA): Observable<ComplianceFormA> {
         let body = JSON.stringify(form);
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });

        return this.http.post(this._baseUrl + 'search/ScanSaveComplianceForm', body, this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }
    
    getInvestigatorSiteSummary(formId: string, investigatorId:number) {

        let headers = new Headers();
        headers.append('Content-Type', 'application/json');

        return this.http.get(this._baseUrl + 'search/GetInvestigatorSiteSummary/?formId=' + formId + "&investigatorId=" + investigatorId, this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }
    
    generateComplianceForm(formId: string){

        let headers = new Headers();
        headers.append('Content-Type', 'application/json');

        return this.http.get(this._baseUrl + 'search/GenerateComplianceForm/?ComplianceFormId=' + formId, this._options )
            .map((res: Response) => {
                return res.json();
            })
            .catch(
                this.handleError
            );
    }
    
    downLoadComplianceForm(formId: string){

        let headers = new Headers();
        headers.append('Content-Type', 'application/json');

        return this.http.get(this._baseUrl + 'search/DownloadComplianceForm/?ComplianceFormId=' + formId, this._options )
            .map((res: Response) => {
                //return res.json();
            })
            .catch(this.handleError);
    }
    
    
    getSiteSources(): Observable<SiteSource[]> {
        
        return this.http.get(this._baseUrl + 'search/GetSiteSources', this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }
    
    //-------------------------

    private extractData(res: Response) {
        let body = res.json();
        return body.data || {};

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