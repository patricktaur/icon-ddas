import { Component, ViewChild, OnChanges } from '@angular/core';
import { Router } from '@angular/router';
import { ActivatedRoute, Params } from '@angular/router';
import { ConfigService } from '../shared/utils/config.service';
import { HelpService } from './help-service';

@Component({
    moduleId: module.id,
    templateUrl: 'help.component.html',

})

export class HelpComponent {
    public generating: boolean = false;
    public downloadUrl: string;
    public OutputGenerationError: string;

    constructor(
        private service: HelpService,
        private route: ActivatedRoute,
        private configService: ConfigService,
    ) {

    }

    ngOnInit(){
        this.downloadUserManual();
    }

    downloadUserManual(){
        this.service.downloadUserManual()
            .subscribe((item : any) => {

            },
            error => {
                
            });        
    }
}