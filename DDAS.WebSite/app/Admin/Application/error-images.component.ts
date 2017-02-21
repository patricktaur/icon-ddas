import { Component, OnInit } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import {LoginHistoryService} from './all-loginhistory.service';
import {  IMyDateModel } from '../../shared/utils/my-date-picker/interfaces';

@Component({
    moduleId: module.id,
    //selector: 'User-input',
    templateUrl: 'error-images.component.html',
})
export class ErrorImagesComponent implements OnInit {
    public ErrorImages: any[];

    constructor(
        private service: LoginHistoryService
    ) { }

    ngOnInit(){
        this.LoadErrorImages();
    }

    LoadErrorImages(){
        this.service.getAllErrorImages()
        .subscribe((item : any[]) => {
            this.ErrorImages = item;
        });   
    }

    deleteAllErrorImages(){
        this.service.deleteAllErrorImages()
        .subscribe((item : any[]) => {
        //this.ErrorImages = item;
        this.LoadErrorImages();
        });
    }
}