import { Component, OnInit, OnDestroy, Input, Output, OnChanges, EventEmitter } from '@angular/core';
import { SearchService } from '../search-service';

@Component({
    selector: '[pick-unassigned-compliance-form]',
    moduleId: module.id,
    templateUrl: 'pick-unassigned-compliance-form.component.html',
})
export class PickUnassignedComplianceFormComponent  {
    constructor(
        private service: SearchService,
    ) { }   

}