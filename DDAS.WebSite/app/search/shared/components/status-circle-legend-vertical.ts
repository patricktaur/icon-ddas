import { Component, OnInit } from '@angular/core';
import { ComplianceFormStatusEnum } from '../../search.classes';

@Component({
    selector: '[statusCircle-Legend-Vertical]',
    template: `
        
<div class="well" >    
    <h4>Legend:</h4>
    <br>
    <div class="text-left">
        <ul style="list-style-type:none; font-size:12px; font-weight: normal; padding-left: 0">
            <li><div statusCircle size=16 [status]="NotScanned" style="float:left;"></div><div >&nbsp;Not scanned.</div><div style="clear: left;"></div>  </li>
            <li><div statusCircle size=16 [status]="NoMatchFoundReviewPending" style="float:left;"></div><div >&nbsp;No match found, Review pending</div><div style="clear: left;"></div>  </li>
            <li><div statusCircle size=16 [status]="FullMatchFoundReviewPending" style="float:left;"></div><div >&nbsp;Matches Found / Issues Identified, Review pending</div><div style="clear: left;"></div>  </li>
            <li><div statusCircle size=16 [status]="ReviewCompletedIssuesNotIdentified" style="float:left;"></div><div >&nbsp;Review Completed, Issues not identified.</div><div style="clear: left;"></div>  </li>
            <li><div statusCircle size=16 [status]="ReviewCompletedIssuesIdentified" style="float:left;"></div><div >&nbsp;Review Completed, Issues identified.</div><div style="clear: left;"></div>  </li>
        </ul>
    </div>
</div>



<!--        <h4>Legend:</h4>
        <ul class="list-group">
            <li class="list-group-item">
                <span class="badge" style="background-color: white"><div statusCircle size=16 [status]="NotScanned" ></div></span>
                Not scanned.
            </li>
            <li class="list-group-item">
                <span class="badge" style="background-color: white"><div statusCircle size=16 [status]="NoMatchFoundReviewPending"></div></span>
                No match found, Review pending.
            </li>
            <li class="list-group-item">
                <span class="badge" style="background-color: white"><div statusCircle size=16 [status]="FullMatchFoundReviewPending"></div></span>
                Full / Partial match found, Review pending.
            </li>
            <li class="list-group-item">
                <span class="badge" style="background-color: white"><div statusCircle size=16 [status]="ReviewCompletedIssuesNotIdentified" ></div></span>
                Review Completed, Issues not identified.
            </li>
            <li class="list-group-item">
                <span class="badge" style="background-color: white"><div statusCircle size=16 [status]="ReviewCompletedIssuesIdentified"></div></span>
                Review Completed, Issues identified.
            </li>
        </ul> -->
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