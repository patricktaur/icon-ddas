import { Component, OnInit } from '@angular/core';
import { ComplianceFormStatusEnum } from '../../search.classes';

@Component({
    selector: '[statusCircle-Legend-Vertical]',
    template: `
        <div class="well " style="padding:0">
        
             
              <div class="row text-center">
              <h4>Legend:</h4>
              </div>
            <div class="row text-left">
   
                <ul style="list-style-type:none; font-size:12px; font-weight: normal;">
                    <li><div statusCircle size=16 [status]="NotScanned" style="float:left;"></div><div >&nbsp;Not scanned.</div><div style="clear: left;"></div>  </li>
                    <li><div statusCircle size=16 [status]="NoMatchFoundReviewPending" style="float:left;"></div><div >&nbsp;No match found, Review pending</div><div style="clear: left;"></div>  </li>
                    <li><div statusCircle size=16 [status]="FullMatchFoundReviewPending" style="float:left;"></div><div >&nbsp;Full / Partial match found, Review pending</div><div style="clear: left;"></div>  </li>
                    <li><div statusCircle size=16 [status]="ReviewCompletedIssuesNotIdentified" style="float:left;"></div><div >&nbsp;Review Completed, Issues not identified.</div><div style="clear: left;"></div>  </li>
                    <li><div statusCircle size=16 [status]="ReviewCompletedIssuesIdentified" style="float:left;"></div><div >&nbsp;Review Completed, Issues identified.</div><div style="clear: left;"></div>  </li>
                
                </ul>
           </div>
        </div>
    `,
})
export class StatusCircleLegendVerticalComponent implements OnInit {
  
   public ReviewCompletedIssuesNotIdentified: number;
     public FullMatchFoundReviewPending: number;
      public NoMatchFoundReviewPending: number;
       public NotScanned: number;
        public PartialMatchFoundReviewPending: number;
         public ReviewCompletedIssuesIdentified: number;

    
    ngOnInit() {
        
       this.NotScanned = ComplianceFormStatusEnum.NotScanned; 
        this.NoMatchFoundReviewPending = ComplianceFormStatusEnum.NoMatchFoundReviewPending;
        //only one is used:
        this.FullMatchFoundReviewPending = ComplianceFormStatusEnum.FullMatchFoundReviewPending;
        this.PartialMatchFoundReviewPending = ComplianceFormStatusEnum.PartialMatchFoundReviewPending;
       
       this.ReviewCompletedIssuesNotIdentified = ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified;       
      
       this.ReviewCompletedIssuesIdentified = ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified;
      
       
       
      
    }

}