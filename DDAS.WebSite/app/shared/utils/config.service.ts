import { Injectable } from '@angular/core';

@Injectable()
export class ConfigService {
    
    _apiURI : string;
    _downloadURI: string;

    constructor() {
       
        //this._apiURI = 'http://localhost:3943/api/'; //dot net core
        
        //server - all servers:  // do not use - does not work for filedownload path correctly
        //this._apiURI = 'api/';
        //this._apiURI = 'http://192.168.137.1/demoddas/api/'; //from server
        //this._apiURI = 'http://ddas.claritytechnologies.com/api'; //from server
        //this._apiURI = 'http://ddasuat.claritytechnologies.com/api/'; //


        //development:
        this._apiURI = 'http://localhost:56846/api/'; //
        
       
        
        //for  demo version on Clarity Server:
        //this._apiURI = 'api/';
        
        
    }

     getApiURI() {
         return this._apiURI;
     }

     getApiHost() {
         return this._apiURI.replace('api/','');
     }

     getVer(){
         return "1.1.0"
     }
}

/*
  Steps to set up local api:
  1. In ConfigService:
    this._apiURI = 'app/';
  2.  In app Module: enable InMemoryWebApiModule.forRoot(SearchData)
      imports: [
        . . .
    
        //comment when using WebAPI Calls
        InMemoryWebApiModule.forRoot(SearchData)

        ],
 3.  In search.service.ts:

       //when using WebAPI calls:
         //return res.json();
       
       //when using local calls:
         let body = res.json();
        return body.data || { };

*/