import { Location } from '@angular/common';
import { Component, OnInit, OnDestroy, NgZone, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';


@Component({
    moduleId: module.id,
    templateUrl: 'logs-main.component.html',
    
})
export class LogsMainComponent implements OnInit {
    public loading: boolean = false;
    public error: any;
    

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        
    ) { }

    ngOnInit() {
        
    }

    

}