import { Component, OnInit, NgZone} from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import {StudyNumbers, SearchList} from './search.classes';
import { ConfigService } from '../shared/utils/config.service';
import { Http, Response, Headers , RequestOptions } from '@angular/http';



@Component({
   moduleId: module.id,
  templateUrl: 'search-input.component.html'  
})
export class SearchInputComponent implements OnInit{ 
  
  public  NameToSearch:string;
  private active:boolean;
  //private StudyNumbers:StudyNumbers[];
  public StudyNumbers:string[];

private zone: NgZone;
  public basicOptions: Object;
  public progress: number = 0;
  public response: any = {};
 
   private SearchQue: SearchList[];

  public uploadUrl: string;
  private error: any;
  
  
  constructor(
       private route: ActivatedRoute,
    private router: Router, private configService: ConfigService
  ) {
    this.NameToSearch="Fiddes, Robert A.";
    this.active=false;
 
  }


  ngOnInit() {
    this.uploadUrl = this.configService.getApiURI() + "search/Upload";
    
    this.zone = new NgZone({ enableLongStackTrace: false });
    this.basicOptions = {
      url: this.uploadUrl
    };


    this.route.params.forEach((params: Params) => {
        this.NameToSearch = params['name'];
        this.StudyNumbers=[];
        this.LoadStudyNumber();
      });
  } 


  LoadStudyNumber(){

  this.StudyNumbers =  ['0000','1111','22222','4444444'
         /* {StudyNumber:'dffdfdfdf'},
          {StudyNumber:'dffdfdfdf'},
          {StudyNumber:'dffdfdfdf'}*/
      /* {StudyNumber:{Name:'Hai',Id:1222}},
        {StudyNumber:{Name:'Hai',Id:1222}},
         {StudyNumber:{Name:'Hai',Id:1222}},*/
  ]

}

  handleUpload(data: any): void {
    console.log("handleUpload");
    this.zone.run(() => {
      this.response = data.response;
   
     
    //    if (data){
    //       data
    //       .map((resp => resp.json().items) 
    //       .subscribe(
    //   data => this.SearchQue = data,
    //   error => console.log(error)
    // );
          
     
  
      
      // return this.http.get(this._baseUrl + 'StudyNumbers')
      //       .map((res: Response) => {
      //           return res.json();
      //       })
      //       .catch(this.handleError);
      
      // data.map(res => {
      //   // If request fails, throw an Error that will be caught
      //   if (res.status < 200 || res.status >= 300) {
      //     throw new Error('This request has failed ' + res.status);
      //   }
      //   // If everything went fine, return the response
      //   else {
      //     this.response = res.json();
      //   }
      // })
      //   .subscribe(
      //   (data) => this.response = data, // Reach here if res.status >= 200 && <= 299
      //   (err) => this.error = err); // Reach here if fails
      this.progress = data.progress.percent / 100;
    });
  }
  
  hack(val) {
    if (val == undefined){
      return null;
    }
    else{
       return val.json();
        //return Array.from(val);
    }
    
  }
  
  
  goToSearch() {
       //this.router.navigate(['summary', this.NameToSearch], { relativeTo: this.route });
       this.router.navigate(['summary', this.NameToSearch], { relativeTo: this.route });
  }
  
 
 get ComplianceFormSummaryList() { 
    //return null;
    
    return this.hack(this.response );
   
  
  //  return [
  //    {NameToSearch: "Aiache, Adrien E.", SearchDate: "1-Jan-2016", MatchSummary : "No match", ProcessedSummary: "2 sites processed"},
  //    {NameToSearch: "Berman, David E.", SearchDate: "12-Jan-2016", MatchSummary : "1 Match found", ProcessedSummary: "8 sites processed"},
  //    {NameToSearch: "Copanos, John D.", SearchDate: "15-Jan-2016", MatchSummary : "1 Match found", ProcessedSummary: "8 sites processed"},
  //  ]
  //   ; 
  }

get diagnostic() { return this.ComplianceFormSummaryList; }
}