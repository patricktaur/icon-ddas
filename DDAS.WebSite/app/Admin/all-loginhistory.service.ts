import { Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
//Grab everything with import 'rxjs/Rx';
import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';

//import { Pagination, PaginatedResult } from '../shared/interfaces';

//import { ConfigService } from '../shared/utils/config.service';
//import { AuthService } from '../auth/auth.service';
import { ConfigService } from '../shared/utils/config.service';
import { AuthService } from '../auth/auth.service';
import {SiteSourceViewModel} from './appAdmin.classes';
import {Country} from './appAdmin.classes';
import {SponsorProtocol} from './appAdmin.classes';
import {DefaultSite} from './appAdmin.classes';
import { DownloadDataFilesViewModel } from './appAdmin.classes';

@Injectable()
export class LoginHistoryService {
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

    getLoginHistory(from:Date, to: Date){
        
                return this.http.get(this._baseUrl + 'Account/getLogHistory'
                + '?DateFrom=' + from.toISOString().substring(0, 10)
                + '&DateTo=' + to.toISOString().substring(0, 10)
                , this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);      
    }

    getAllErrorImages(){
        return this.http.get(this._baseUrl + 'AppAdmin/GetErrorImages',
        this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);
    }

    getAllDataFiles(siteEnum: number):Observable<DownloadDataFilesViewModel[]>{
        return this.http.get(this._baseUrl + 'Search/DownloadDataFiles?SiteEnum=' + siteEnum,
        this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);        
    }

    deleteErrorImage(FileName: string){
        return this.http.get(this._baseUrl + 'AppAdmin/DeleteErrorImage?FileName=' + FileName,
        this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);
    }

    deleteAllErrorImages(){
        return this.http.get(this._baseUrl + 'AppAdmin/DeleteAllErrorImages',
        this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);
    }

    getDataExtractionPerSite(SiteEnum:number){
        return this.http.get(this._baseUrl + 'AppAdmin/GetDataExtractionPerSite?Enum=' + SiteEnum,
        this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);
    }

    getExtractionHistory(){
        return this.http.get(this._baseUrl + 'admin/GetDataExtractionHistory',
        this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);
    }

    deleteExtractionData(RecId: string, Enum: number){
        return this.http.get(this._baseUrl + 'AppAdmin/DeleteExtractionData?RecId=' + RecId
        + '&Enum=' + Enum,
        this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);
    }

    getErrorImageFolderPath(){
        return this.http.get(this._baseUrl + 'AppAdmin/GetErrorScreenCaptureFolderPath',
        this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);  
    }

    getSiteSources(){
        return this.http.get(this._baseUrl + 'admin/GetSiteSources',
        this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);
    }

    getSiteSource(RecId: string){
        return this.http.get(this._baseUrl + 'admin/GetSiteSource?RecId=' + RecId, this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);
    }

    saveSiteSource(SiteSource: SiteSourceViewModel): Observable<SiteSourceViewModel>{
        let body = JSON.stringify(SiteSource);
        return this.http.post(this._baseUrl + 'admin/SaveSiteSource/', body, this._options)
                   .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

   deleteSiteSource(RecId: string){
        return this.http.get(this._baseUrl + 'admin/DeleteSiteSource?RecId=' + RecId,
        this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);
    }
    
    getCountries(){
        return this.http.get(this._baseUrl + 'admin/GetCountries',
        this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);
    }

    getCountry(RecId: string){
        return this.http.get(this._baseUrl + 'admin/GetCountry?RecId=' + RecId,
        this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);        
    }

    saveCountry(countryList: Country){
        let body = JSON.stringify(countryList);
        return this.http.post(this._baseUrl + 'admin/AddCountry/', body, this._options)
                   .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

    removeCountry(RecId: string){
        return this.http.get(this._baseUrl + 'admin/DeleteCountry?RecId=' + RecId,
        this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);
    }
    
    saveSponsorProtocol(sponsorProtocol : SponsorProtocol){
        let body = JSON.stringify(sponsorProtocol);
        return this.http.post(this._baseUrl + 'admin/AddSponsorProtocol/', body, this._options)
                   .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

    getSponsorProtocols(){
        return this.http.get(this._baseUrl + 'admin/GetSponsorProtocols',
        this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);        
    }

    getSponsorProtocol(RecId: string){
        return this.http.get(this._baseUrl + 'admin/GetSponsorProtocol?RecId=' + RecId,
        this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);
    }

    removeSponsorProtocol(RecId: string){
        return this.http.get(this._baseUrl + 'admin/DeleteSponsorProtocol?RecId=' + RecId,
        this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);
    }

     addDefaultSite(defaultSite : DefaultSite){
        let body = JSON.stringify(defaultSite);
        return this.http.post(this._baseUrl + 'admin/AddDefaultSite/', body, this._options)
                   .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

    getDefaultSites(){
        return this.http.get(this._baseUrl + 'admin/GetDefaultSites',
        this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);        
    }

    getDefaultSite(RecId: string){
        return this.http.get(this._baseUrl + 'admin/GetDefaultSite/?RecId=' + RecId,
        this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);        
    }
     
    saveDefaultSite(SiteSource: DefaultSite): Observable<DefaultSite>{
        let body = JSON.stringify(SiteSource);
        return this.http.post(this._baseUrl + 'admin/SaveDefaultSite/', body, this._options)
                   .map((res: Response) => {
                return res.json();
            })
        .catch(this.handleError);
    }
     
     
     removeDefaultSite(RecId: string){
        return this.http.get(this._baseUrl + 'admin/DeleteDefaultSite?RecId=' + RecId,
        this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);
    }
    
    getExceptionLogs(){
        return this.http.get(this._baseUrl + 'admin/GetExceptionLogs', this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);
    }
    
    getExtractionLog(){
        return this.http.get(this._baseUrl + 'admin/GetExtractionLog', this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);
    }

    getISprintToDDASLog(){
        return this.http.get(this._baseUrl + 'admin/iSprintToDDASLog', this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);
    }

    getDDAStoiSprintLog(){
        return this.http.get(this._baseUrl + 'admin/DDAStoiSprintLog', this._options)
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