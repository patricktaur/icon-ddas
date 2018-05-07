import { Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions, ResponseContentType } from '@angular/http';
//Grab everything with import 'rxjs/Rx';
import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { ConfigService } from '../shared/utils/config.service';
import { AuthService } from '../auth/auth.service';
import { QualityCheck } from './qc.classes';
import { ComplianceFormA, CompFormFilter } from '../search/search.classes';

@Injectable()
export class QCService {
    _baseUrl: string = '';
    _controller: string = '';
    _options: RequestOptions;

    constructor(private http: Http,
        private configService: ConfigService,
        private authService: AuthService) {
        this._baseUrl = configService.getApiURI() + this._controller;
        let headers = new Headers();
        headers.append('Content-Type', 'application/json');
        headers.append("Authorization", "Bearer " + this.authService.token);
        this._options = new RequestOptions({ headers: headers });
    }

    listQCs(filter: CompFormFilter) {
        let filterJSON = JSON.stringify(filter);
        return this.http.post(this._baseUrl + 'QC/ListQCs', filterJSON, this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);
    }

    loadQC(AuditId: string){
        return this.http.get(this._baseUrl + 'QC/GetQC?Id=' + AuditId, this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);        
    }

    getQC(formId: string, qcAssignedTo: string){
        return this.http.get(this._baseUrl + 'QC/GetQC?Id='
        + formId + '&AssignedTo=' + qcAssignedTo, this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);        
    }

    getComplianceForm(formId: string): Observable<any> {
        return this.http.get(this._baseUrl + 'search/GetComplianceForm?formId=' + formId, this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

    getAttachmentsList(formId: string): Observable<any> {
        return this.http.get(this._baseUrl + 'search/GetAttachmentsList?formId=' + formId, this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

    submitQC(compForm: ComplianceFormA): Observable<ComplianceFormA>{
        let body = JSON.stringify(compForm);
        return this.http.post(this._baseUrl + 'QC/SubmitQC', body, this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);
    }

    listQCSummary(complianceFormId: string): Observable<any[]>{
        return this.http.get(this._baseUrl + 'QC/ListQCSummary?FormId=' + complianceFormId, this._options)
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