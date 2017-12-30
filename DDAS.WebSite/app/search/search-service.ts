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
    QualityCheck
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

        return this.http.get(this._baseUrl + 'search/DeleteComplianceForm?ComplianceFormId=' + CompFormId, this._options)
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

        return this.http.post(this._baseUrl + 'search/ComplianceFormFilters', Filter1, this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);

    }

    getAllUsers() {

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

    getClosedComplianceFormFilters(Filters: CompFormFilter): Observable<PrincipalInvestigatorDetails[]>{
        var compFormFilter = JSON.stringify(Filters);
        return this.http.post(this._baseUrl + 'search/ClosedComplianceFormFilters', compFormFilter, this._options)
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

        return this.http.get(this._baseUrl + 'search/GetComplianceForm?formId=' + formId, this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

    getUploadsFolderPath() {
        return this.http.get(this._baseUrl + 'search/GetUploadsFolderPath', this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

    getUploadedFile(generatedFileName: string, originalFileName: string){
        let headers = new Headers();
        headers.append("Authorization", "Bearer " + this.authService.token);
        headers.append('Content-Type', 'application/json');

        let file = {};
        return this.http.get(this._baseUrl + 'Search/DownloadUploadedFile?GeneratedFileName=' + generatedFileName,
            { headers: headers, responseType: ResponseContentType.ArrayBuffer })
            .map((res: Response) => {
                // return res.json();
                file = new Blob([res.arrayBuffer()], {
                    type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
                });

                var browser = res.headers.get('Browser');

                console.log("Original Filename: " + originalFileName);
                console.log("Browser: " + browser);

                if (browser.toLowerCase() == "edge" ||
                    browser.toLowerCase() == "ie") {
                    window.navigator.msSaveBlob(file, originalFileName);
                }

                if (browser.toLowerCase() == "chrome") {
                    var anchor = document.createElement("a");
                    anchor.download = originalFileName;
                    anchor.text = originalFileName;
                    anchor.href = window.URL.createObjectURL(file, originalFileName);
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

    SaveAssignedTo(AssignedTo: string, Active: boolean, ComplianceFormId: string): Observable<boolean> {
        return this.http.get(this._baseUrl + 'search/SaveAssignedToData?' +
            'AssignedTo=' + AssignedTo + '&Active=' + Active + '&ComplianceFormId=' + ComplianceFormId,
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

    getInvestigatorSiteSummary(formId: string, investigatorId: number) {

        let headers = new Headers();
        headers.append('Content-Type', 'application/json');

        return this.http.get(this._baseUrl + 'search/GetInvestigatorSiteSummary?formId=' + formId + "&investigatorId=" + investigatorId, this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

    getInstituteFindingsSummary(formId: string) {

        let headers = new Headers();
        headers.append('Content-Type', 'application/json');

        return this.http.get(this._baseUrl + 'search/getInstituteFindingsSummary?formId=' + formId, this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

    generateComplianceForm(formId: string) {
        // let headers = new Headers();
        // headers.append('Content-Type', 'application/json');

        // return this.http.get(this._baseUrl + 'search/GenerateComplianceForm?ComplianceFormId=' + formId, this._options)
        //     .map((res: Response) => {
        //         return res.json();
        //     })
        //     .catch(
        //     this.handleError
        //     );

        let headers = new Headers();
        headers.append("Authorization", "Bearer " + this.authService.token);
        headers.append('Content-Type', 'application/json');

        let file = {};
        return this.http.get(this._baseUrl + 'Search/GenerateComplianceForm?ComplianceFormId=' + formId,
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

                console.log("Filename header: " + fileNameHeader);
                console.log("File Name: " + fileName);
                console.log("Browser: " + browser);

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

    generateComplianceFormPDF(formId: string) {
        let headers = new Headers();
        headers.append("Authorization", "Bearer " + this.authService.token);
        headers.append('Content-Type', 'application/json');

        let file = {};
        return this.http.get(this._baseUrl + 'Search/GenerateComplianceFormPDF?ComplianceFormId=' + formId,
            { headers: headers, responseType: ResponseContentType.ArrayBuffer })
            .map((res: Response) => {
                // return res.json();
                file = new Blob([res.arrayBuffer()], {
                    type: 'application/pdf'
                });

                //header 'Browser' in the response is not read by Microsoft 'Edge'. Not sure why
                //hence the work around of 'split with space'!
                // var browser = res.headers.get('Browser');
                var fileNameHeader = res.headers.get('Filename');
                var fileName = fileNameHeader.split(' ')[0].trim();
                // var browser = res.headers.get('Browser');
                var browser = fileNameHeader.split(' ')[1].trim();

                console.log("Filename header: " + fileNameHeader);
                console.log("File Name: " + fileName);
                console.log("Browser: " + browser);

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
        let headers = new Headers();
        headers.append('Content-Type', 'application/json');

        return this.http.get(this._baseUrl + 'search/GenerateOutputFile', this._options)
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
        return this.http.get(this._baseUrl + 'Search/GenerateComplianceForm?ComplianceFormId=' + formId,
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

    getSiteSources(): Observable<SiteSource[]> {

        return this.http.get(this._baseUrl + 'search/GetSiteSources', this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

    requestQC(form: ComplianceFormA){
        let body = JSON.stringify(form);
        let headers = new Headers({'Content-Type': 'application/json'});
        let options = new RequestOptions({headers: headers});

        return this.http.post(this._baseUrl + 'QC/RequestQC', body, this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

    saveCompFormGeneralNInvestigatorsNOptionalSites(form: ComplianceFormA) {

        let body = JSON.stringify(form);
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });

        return this.http.post(this._baseUrl + 'search/UpdateCompFormGeneralNInvestigators', body, this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);

    }

    saveFindingsAndObservations(updateFindings: UpdateFindigs) {

        let body = JSON.stringify(updateFindings);
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });

        return this.http.post(this._baseUrl + 'search/UpdateFindings', body, this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

    //UpdateInstituteFindings(string FormId, int SiteDisplayPosition, List<Finding> Findings)
    updateInstituteFindings(findingsModel: UpdateInstituteFindings) {

        let body = JSON.stringify(findingsModel);
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });

        return this.http.post(this._baseUrl + 'search/UpdateInstituteFindings', body, this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

    getSingleComponentMatchedRecords(SiteDataId: string, SiteEnum: number, FullName: string) {
        return this.http.get(this._baseUrl + 'search/GetSingleComponentMatchedRecords?SiteDataId=' + SiteDataId
            + '&SiteEnum=' + SiteEnum
            + '&FullName=' + FullName,
            this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);

    }
    //string SiteDataId, SiteEnum Enum, string FullName

    //-------------------------

    private extractData(res: Response) {
        let body = res.json();
        return body.data || {};
    }

    downloadComplianceFormTest() {
        let headers = new Headers();
        //headers.append('Content-Type', 'application/json');
        headers.append("Authorization", "Bearer " + this.authService.token);
        //let options = new RequestOptions({ headers: headers });
        let file = {};
        return this.http.get(this._baseUrl + 'search/GenerateOutputFile/',
            { headers: headers, responseType: ResponseContentType.ArrayBuffer })
            .map((res: Response) => {
                //return res;
                file = new Blob([res.arrayBuffer()], {
                    type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
                });
                var filename = res.headers.get('FileName');
                console.log("Downloaded filename: " + filename);
                var anchor = document.createElement("a");
                anchor.download = filename;
                anchor.href = window.URL.createObjectURL(file);
                anchor.click();
                //window.open(window.URL.createObjectURL(file));
            })
            .catch(this.handleError);
    }

    // .map(res => new Blob([res.arrayBuffer()], {
    //     type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
    // }))
    // .catch(this.handleError);

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