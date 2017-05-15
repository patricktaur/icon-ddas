import { Component, OnInit } from '@angular/core';
import { ComplianceFormStatusEnum } from '../../search.classes';

@Component({
    selector: '[statusCircle-Legend-Horizontal]',
    template: `
      <div class="well " style="padding:5px">

	<div class="row">

		<div class="col-md-2 ">
			<h4>Legend:</h4>
		</div>
		<div class="col-md-2 text-center">
			<div class="row">
				<div class="col-md-12 ">
					<div statusCircle size=16 [status]="NotScanned"></div>
				</div>
			</div>
			<div class="row">
				<div class="col-md-12 ">
					Not scanned.
				</div>
			</div>
		</div>
		<div class="col-md-2 text-center">
			<div class="row">
				<div class="col-md-12 ">
					<div statusCircle size=16 [status]="NoMatchFoundReviewPending"></div>
				</div>
			</div>
			<div class="row">
				<div class="col-md-12 ">
					No match found, Review pending
				</div>
			</div>
		</div>

		<div class="col-md-2 text-center">
			<div class="row">
				<div class="col-md-12 ">
					<div statusCircle size=16 [status]="FullMatchFoundReviewPending"></div>
				</div>
			</div>
			<div class="row">
				<div class="col-md-12 ">
					Matches Found / Issues Identified, Review pending
				</div>
			</div>
		</div>


		<div class="col-md-2 text-center">
			<div class="row">
				<div class="col-md-12 ">
					<div statusCircle size=16 [status]="ReviewCompletedIssuesNotIdentified"></div>
				</div>
			</div>
			<div class="row">
				<div class="col-md-12 ">
					Review Completed, Issues not identified.
				</div>
			</div>
		</div>

		<div class="col-md-2 text-center">
			<div class="row">
				<div class="col-md-12 ">
					<div statusCircle size=16 [status]="ReviewCompletedIssuesIdentified"></div>
				</div>
			</div>
			<div class="row">
				<div class="col-md-12 ">
					Review Completed, Issues identified.
				</div>
			</div>
		</div>

	</div>

</div>
    `,
})
export class StatusCircleLegHorComponent implements OnInit {

    public ReviewCompletedIssuesNotIdentified: number;
    public FullMatchFoundReviewPending: number;
    public NoMatchFoundReviewPending: number;
    public NotScanned: number;
    public PartialMatchFoundReviewPending: number;
	public SingleMatchFoundReviewPending: number;
    public ReviewCompletedIssuesIdentified: number;

    ngOnInit() {
        this.NotScanned = ComplianceFormStatusEnum.NotScanned;
        this.NoMatchFoundReviewPending = ComplianceFormStatusEnum.NoMatchFoundReviewPending;
        //only one is used:
        this.FullMatchFoundReviewPending = ComplianceFormStatusEnum.FullMatchFoundReviewPending;
        this.PartialMatchFoundReviewPending = ComplianceFormStatusEnum.PartialMatchFoundReviewPending;
		this.SingleMatchFoundReviewPending = ComplianceFormStatusEnum.SingleMatchFoundReviewPending;
        this.ReviewCompletedIssuesNotIdentified = ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified;

        this.ReviewCompletedIssuesIdentified = ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified;


    }

}