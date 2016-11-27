import { Component, ViewContainerRef,OnInit, OnDestroy, NgZone} from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import {StudyNumbers, SearchList, ComplianceForm} from './search.classes';
import { ConfigService } from '../shared/utils/config.service';
import { Http, Response, Headers , RequestOptions } from '@angular/http';
import {SearchService} from './search-service';
import {Observable, Subscription} from 'rxjs/Rx';

// import { Modal } from 'angular2-modal/plugins/bootstrap';

@Component({
   moduleId: module.id,
  templateUrl: 'search-input.component.html',
  //styleUrls: ['./stylesTable.css']  
})
export class SearchInputComponent implements OnInit{ 
  
  public  NameToSearch:string;
  private active:boolean;
  //private StudyNumbers:StudyNumbers[];
  public StudyNumbers:string[];
  private CompForms: ComplianceForm[];
  public Processing :boolean = false;
  public Loading :boolean = false;

  private zone: NgZone;
  public basicOptions: Object;
  public progress: number = 0;
  public response: any = {};
 
   private SearchQue: SearchList[];

  public uploadUrl: string;
  private error: any;
  public TodeleteRecID :string;
  private  subscription : Subscription ;
  constructor(
       private route: ActivatedRoute,
      private router: Router, 
      private configService: ConfigService,
      private service: SearchService,
      // vcRef: ViewContainerRef, public modal: Modal
  ) {
    this.NameToSearch="Fiddes, Robert A.";
    this.active=false;
     //modal.confirm();
 
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

       this.StartRefresh();
  } 

  ngOnDestroy(){
    this.StopRefresh();
  }

   RefreshList(){
        this.LoadNamesFromOpenComplianceForm();
    }

    StartRefresh(){
        console.log("StartRefresh");
        this.subscription =  Observable.interval(500 * 60).subscribe(x => {
            this.LoadNamesFromOpenComplianceForm()
        });
    }

    //*****
    //Not used, required later:
    StopRefresh(){
      console.log("StopRefresh");
      if (this.subscription != null && !this.subscription.closed) {
                this.subscription.unsubscribe();
            }
    }

LoadStudyNumber(){

  this.StudyNumbers =  ['0000','1111','22222','4444444']

}

LoadNamesFromOpenComplianceForm()
{
  
  this.Processing = false ;
  this.service.getNamesFromOpenComplianceForm()
            .subscribe((item: any) => {
                this.CompForms = item;
                this.Loading = false ;
                 console.log('Loaded ComplianceForm Data ' + new Date());
                  },
            error => {
               console.log('Error : Load ComplianceForm');
            });
            console.log('Completed Load ComplianceForm');
}
  
handleUpload(data: any): void {
    this.Loading = true ;
    console.log("handleUpload");
    this.zone.run(() => {
      this.response = data.response;
      this.LoadNamesFromOpenComplianceForm(); 
      console.log(data);
      this.progress = data.progress.percent / 100;
      //this.Loading = false ;
    });

     
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
  //  ]; 
  }
  goToSearchAdd(NameToSearch : string){
    this.Processing = true;
    console.log('Add new Name  :  ' + NameToSearch)
    this.service.getSingleNameUpdatedNamesFromOpenComplianceForm(NameToSearch)
      .subscribe((item: any) => {
          //this.CompForms = item;
          console.log(item);
          this.LoadNamesFromOpenComplianceForm();
          this.NameToSearch = undefined;
            },
      error => {

    });
    //this.LoadNamesFromOpenComplianceForm();
    //this.router.navigate(['/search']);
  }

gotoSummaryResult(DataItem : ComplianceForm){
   
   this.router.navigate(['summary',DataItem.NameToSearch,DataItem.RecId,DataItem.Sites_FullMatchCount,
   DataItem.Sites_PartialMatchCount ], { relativeTo: this.route });
 
}

CloseReviewCompForm(RecId : string){
   this.service.CloseReviewComplianceForm(RecId)
            .subscribe((item: any) => {
               this.LoadNamesFromOpenComplianceForm();
            },
            error => {

            });
}


ToDelete(CompFormId : string){
     console.log('delete function');
     this.TodeleteRecID=CompFormId;
      this.service.deleteDataFromComplianceForm(this.TodeleteRecID)
      .subscribe((item: any) => {
          this.LoadNamesFromOpenComplianceForm();
             },
      error => {

    });
}

MatchCountHighlight(MatchCount:number){
    if (MatchCount==0){
      return true;
    }
    else{
      return false;
    }
}
  
  // onClick() {
  //   this.modal.alert()
  //       .size('lg')
  //       .showClose(true)
  //       .title('A simple Alert style modal window')
  //       .body(`
  //           <h4>Alert is a classic (title/body/footer) 1 button modal window that 
  //           does not block.</h4>
  //           <b>Configuration:</b>
  //           <ul>
  //               <li>Non blocking (click anywhere outside to dismiss)</li>
  //               <li>Size large</li>
  //               <li>Dismissed with default keyboard key (ESC)</li>
  //               <li>Close wth button click</li>
  //               <li>HTML content</li>
  //           </ul>`)
  //       .open();
  // }



get diagnostic() {return JSON.stringify(this.CompForms);  }
}