import { Component, OnInit } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import {LoginHistoryService} from './all-loginhistory.service';
import {  IMyDateModel } from '../shared/utils/my-date-picker/interfaces';
import { ConfigService } from '../shared/utils/config.service';
import {SiteSourceViewModel} from './appAdmin.classes';


@Component({
    moduleId: module.id,
    //selector: 'User-input',
    templateUrl: 'manage-site-sources.component.html',
})

export class ManageSiteSourcesComponent implements OnInit {
    public SiteSources: SiteSourceViewModel[];
    public ApiHost: string;
    public pageNumber: number;
    public formLoading: boolean;
    public filterSiteURL: string = "";
    private selectedRecId: string;
    public selectedRecordName: string;
    public error: any;
    constructor(
        private service: LoginHistoryService,
        private configService: ConfigService,
        private router: Router, 
        private route: ActivatedRoute
    ) { }

    ngOnInit(){
        this.LoadSiteSources();
    }

    LoadSiteSources(){
        // this.formLoading = false;
        this.service.getSiteSources()
        .subscribe((item : any[]) => {
            this.SiteSources = item;
            // this.formLoading = true;
        },
        error => {
            // this.formLoading = false;
        });
    }

    get filteredRecords(){
        //return this.SiteSources;
        
        if (this.SiteSources == null){
           return this.SiteSources;
        }
        else{
           if (this.filterSiteURL.trim().length > 0){
               //return this.SiteSources;
               
               return this.SiteSources.filter(x => x.SiteUrl.indexOf(this.filterSiteURL.trim() ) > 0);
               //return this.SiteSources.filter(x => x.SiteUrl.indexOf(this.filterSiteURL.trim() ) > 0);
           }
           else{
               return this.SiteSources;
           }
            
        }
        
    }

    extractionModeIsManual(extractionMode: string){
        return (extractionMode.toLowerCase() == "manual");
    }
    
    EditSiteDetails(RecId: string){
        this.router.navigate(['edit-site-source', RecId], { relativeTo: this.route.parent});
    }

    Add(){
        this.router.navigate(['edit-site-source', ""], { relativeTo: this.route.parent});
    }

  setSelectedRecordDetails(rec: SiteSourceViewModel){
       this.selectedRecId = rec.RecId;
       this.selectedRecordName = rec.SiteName;   
   }
   
   Delete(){ 
      //CompFormId to be set by the delete button
      this.error = "";
      this.service.deleteSiteSource(this.selectedRecId)
            .subscribe((item: any) => {
              this.LoadSiteSources();
            },
            error => {
                this.error = "Unable to delete " + this.selectedRecordName + " : " + error;
            });
   }

}