
import { Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions, ResponseContentType } from '@angular/http';
//Grab everything with import 'rxjs/Rx';
import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/Rx';

import { Pagination, PaginatedResult } from '../shared/interfaces';
import {
    SiteInfo, StudyNumbers, SearchSummaryItem, SearchSummary, NameSearch,
    SearchResultSaveData,
    //Site, 
    ComplianceFormA,
    SiteSource,
    CompFormFilter,
    PrincipalInvestigatorDetails,
    Finding,
    UpdateFindigs,
    UpdateInstituteFindings,
    QualityCheck,
    CurrentReviewStatusViewModel,
    UndoEnum,
    Review,
    AssignComplianceFormsTo
} from '../search/search.classes';

//import {FDADebarPageSiteData} from './detail-classes/FDADebarPageSiteData';
//import {NameSearchSiteDetails5} from './site-result-detail.class-5';


import { ConfigService } from '../shared/utils/config.service';
import { AuthService } from '../auth/auth.service';

@Injectable()
export class ComplianceFormArchiveService {
    _baseUrl: string = '';
    //_controller: string = 'archive/'; 
    _controller: string = '';
    _options: RequestOptions;

    constructor(private http: Http,
        private configService: ConfigService,
        private authService: AuthService
    ) {
        this._baseUrl = configService.getApiURI() + this._controller;
        let headers = new Headers();
        headers.append('Content-Type', 'application/json');
        headers.append("Authorization", "Bearer " + this.authService.token);
        this._options = new RequestOptions({ headers: headers });
    }

    getStudyNumbers(): Observable<StudyNumbers[]> {
        return this.http.get(this._baseUrl + 'StudyNumbers')
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

    CloseComplianceForm(RecId: string) {
        return this.http.put(this._baseUrl + 'archive/CloseComplianceForm?ComplianceFormId=' + RecId, null, this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

    OpenComplianceForm(RecId: string) {
        return this.http.put(this._baseUrl + 'archive/OpenComplianceForm?ComplianceFormId=' + RecId, null, this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

    deleteComplianceForm(CompFormId: string) {
        return this.http.get(this._baseUrl + 'archive/DeleteComplianceForm?ComplianceFormId=' + CompFormId, this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

    getPrincipalInvestigators() {
        return this.http.get(this._baseUrl + 'archive/GetPrincipalInvestigators', this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

    


    getPrincipalInvestigatorsByFilters(Filters: CompFormFilter): Observable<PrincipalInvestigatorDetails[]> {
        let Filter1 = JSON.stringify(Filters);
        return this.http.post(this._baseUrl + 'archive/ComplianceFormFilters', Filter1, this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

    getPrincipalInvestigatorsWithReviewDateFilters(Filters: CompFormFilter): Observable<PrincipalInvestigatorDetails[]> {
        let Filter1 = JSON.stringify(Filters);
        return this.http.post(this._baseUrl + 'archive/ComplianceFormWithReviewDateFilters', Filter1, this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }
    
    getAllUsers() {
        return this.http.get(this._baseUrl + 'Account//GetUsers', this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

    

    

    generateComplianceForm(formId: string) {
        let headers = new Headers();
        headers.append("Authorization", "Bearer " + this.authService.token);
        headers.append('Content-Type', 'application/json');

        let file = {};
        return this.http.get(this._baseUrl + 'archive/GenerateComplianceForm?ComplianceFormId=' + formId,
            { headers: headers, responseType: ResponseContentType.ArrayBuffer })
            .map((res: Response) => {
                // return res.json();
                file = new Blob([res.arrayBuffer()], {
                    type: 'application/ms-word'
                });

                //header 'Browser' in the response is not read by Microsoft 'Edge'. Not sure why
                //hence the work around of 'split with space'!
                // var browser = res.headers.get('Browser');
                var fileNameHeader = res.headers.get('Filename');
                var fileName = fileNameHeader.split(' ')[0].trim();
                // var browser = res.headers.get('Browser');
                var browser = fileNameHeader.split(' ')[1].trim();

                // console.log("Filename header: " + fileNameHeader);
                // console.log("File Name: " + fileName);
                // console.log("Browser: " + browser);

                if (browser.toLowerCase() == "edge" ||
                    browser.toLowerCase() == "ie") {
                    window.navigator.msSaveBlob(file, fileName);
                }

                if (browser.toLowerCase() == "chrome") {
                    var anchor = document.createElement("a");
                    anchor.download = fileName;
                    anchor.text = fileName;
                    anchor.href = window.URL.createObjectURL(file, fileName);
                    anchor.click();
                }
                if (browser.toLowerCase() == "unknown") {
                    alert("could not identify the browser. File download failed");
                }
                if (browser == null) {
                    //...
                }
                ////window.open(window.URL.createObjectURL(file));
            })
            .catch(this.handleError);
    }

    


    generateOutputFile() {
        return this.http.get(this._baseUrl + 'archive/GenerateOutputFile', this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(
            this.handleError
            );
    }

    downLoadComplianceForm(formId: string) {
        let headers = new Headers();
        headers.append("Authorization", "Bearer " + this.authService.token);
        headers.append('Content-Type', 'application/json');

        let file = {};
        return this.http.get(this._baseUrl + 'archive/GenerateComplianceForm?ComplianceFormId=' + formId,
            { headers: headers, responseType: ResponseContentType.ArrayBuffer })
            .map((res: Response) => {
                file = new Blob([res.arrayBuffer()], {
                    type: 'application/ms-word'
                });
                var filename = res.headers.get('Filename');
                var browser = res.headers.get('Browser');

                console.log("Downloaded filename: " + filename);
                console.log("Browser: " + browser);

                if (browser.toLowerCase() == "edge" ||
                    browser.toLowerCase() == "ie") {
                    window.navigator.msSaveOrOpenBlob(file, filename);
                }

                if (browser.toLowerCase() == "chrome") {
                    var anchor = document.createElement("a");
                    anchor.download = filename;
                    anchor.href = window.URL.createObjectURL(file);
                    anchor.click();
                }
                if (browser.toLowerCase() == "unknown") {
                    alert("could not identify the browser. File donwload failed");
                }
                if (browser == null) {
                    //alert("Error. could not download file for browser: " + browser);
                }
                //window.open(window.URL.createObjectURL(file));
            })
            .catch(this.handleError);
    }

    
    getUserFullName(userName: string){
        return this.http.get(this._baseUrl + 'archive/getUserFullName?userName=' + userName, this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);
    }

    archiveComplianceFormWithSearchedOnGreaterthan(days: number, limit : number) {
            
            return this.http.get(this._baseUrl + 'archive/ArchiveCompFormsWithSearchDaysGreaterThan?days=' + days + '&limit=' + limit, this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    

        }   

        undoArchive(recId: string): Observable<string> {
            return this.http.get(this._baseUrl + 'archive/UndoArchive?RecId=' + recId, this._options)
                .map((res: Response) => {
                    return res.json();
                })
                .catch(this.handleError);
        }    
    
    private handleError(error: any) {
        var applicationError = error.headers.get('Application-Error');
        var serverError = error.json();
        var modelStateErrors: string = '';
        console.log("serverError:" +JSON.stringify(serverError));
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