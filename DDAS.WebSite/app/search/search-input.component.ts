import { Component, OnInit, NgZone} from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import {StudyNumbers, SearchList, ComplianceForm} from './search.classes';
import { ConfigService } from '../shared/utils/config.service';
import { Http, Response, Headers , RequestOptions } from '@angular/http';
import {SearchService} from './search-service';

@Component({
   moduleId: module.id,
  templateUrl: 'search-input.component.html',
  styleUrls: ['./stylesTable.css']  
})
export class SearchInputComponent implements OnInit{ 
  
  public  NameToSearch:string;
  private active:boolean;
  //private StudyNumbers:StudyNumbers[];
  public StudyNumbers:string[];
  private CompForms: ComplianceForm[];


private zone: NgZone;
  public basicOptions: Object;
  public progress: number = 0;
  public response: any = {};
 
   private SearchQue: SearchList[];

  public uploadUrl: string;
  private error: any;
  
  
  constructor(
    private route: ActivatedRoute,
    private router: Router, 
    private configService: ConfigService,
    private service: SearchService
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

    this.LoadNamesFromOpenComplianceForm(); 
    
    this.route.params.forEach((params: Params) => {
        this.NameToSearch = params['name'];
        this.StudyNumbers=[];
        this.LoadStudyNumber();
      });
  } 


  LoadStudyNumber(){

  this.StudyNumbers =  ['0000','1111','22222','4444444']

}

LoadNamesFromOpenComplianceForm()
{
  console.log("LoadNamesFromOpenComplianceForm called");
  this.service.getNamesFromOpenComplianceForm()
            .subscribe((item: any) => {
                this.CompForms = item;
                  },
            error => {
            });
}

 handleUpload(data: any): void {
    console.log("handleUpload");
    this.zone.run(() => {
      this.response = data.response;
       
        console.log(data);
   
      this.progress = data.progress.percent / 100;
    });
    this.LoadNamesFromOpenComplianceForm(); 
    
  }
  
  goRefreshList(){


  }
    
  goToSearch() {
       //this.router.navigate(['summary', this.NameToSearch], { relativeTo: this.route });
       this.router.navigate(['summary', this.NameToSearch], { relativeTo: this.route });
  }
  
 
 
 
 get ComplianceFormSummaryList() { 
    return this.CompForms;
 
  //  return [
  //    {NameToSearch: "Aiache, Adrien E.", SearchStartedOn: "1-Jan-2016", ProjectNumber : "No match", Country: "2 sites processed", Sites_FullMatchCount : "1", Sites_PartialMatchCount : "2", ComplianceFormId: "1"},
  //    {NameToSearch: "Berman, David E.", SearchStartedOn: "12-Jan-2016", ProjectNumber : "1 Match found", Country: "8 sites processed", Sites_FullMatchCount : "1", Sites_PartialMatchCount : "2", ComplianceFormId: "2"}, 
  //    {NameToSearch: "Copanos, John D.", SearchStartedOn: "15-Jan-2016", ProjectNumber : "1 Match found", Country: "8 sites processed", Sites_FullMatchCount : "1", Sites_PartialMatchCount : "2", ComplianceFormId: "3"},
  //  ]
  //   ; 
  }
  goToSearchAdd(){
    this.router.navigate(['/search']);
  }

gotoSummaryResult(CompFormId: string){
   
   this.router.navigate(['summary', CompFormId], { relativeTo: this.route });
 
}

get diagnostic() {return JSON.stringify(this.CompForms);  }
}