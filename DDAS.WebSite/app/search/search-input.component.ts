import { Component} from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import {StudyNumbers} from './search.classes';

//import { AuthService }      from '../auth/auth.service';
@Component({
   moduleId: module.id,
  templateUrl: 'search-input.component.html'  
})
export class SearchInputComponent { 
  private  NameToSearch:string;
  private active:boolean;
  private Token:string;
  private StudyNumbers:string[];
  constructor(
       private route: ActivatedRoute,
    private router: Router   //,
    //private authservice: AuthService
  ) {
    this.NameToSearch="";
    this.active=false;
  }


  ngOnInit() {
    this.route.params.forEach((params: Params) => {
        this.NameToSearch = params['name'];
        this.StudyNumbers=[];
        this.LoadStudyNumber();
      });
      //this.Token=this.authservice.getToken();
      //  console.log('token in FDA Site' + this.Token);
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

  goToSearch() {
       //this.router.navigate(['summary', this.NameToSearch], { relativeTo: this.route });
       this.router.navigate(['summary', this.NameToSearch], { relativeTo: this.route });
  }

  onChange(value){
    alert(JSON.stringify(value));
    console.log(value);
    console.log(this.active);
  }
}