/// <reference types="core-js" />
import { OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ConfigService } from '../shared/utils/config.service';
export declare class SearchInputComponent implements OnInit {
    private route;
    private router;
    private configService;
    NameToSearch: string;
    private active;
    StudyNumbers: string[];
    private zone;
    basicOptions: Object;
    progress: number;
    response: any;
    private uploadUrl;
    constructor(route: ActivatedRoute, router: Router, configService: ConfigService);
    ngOnInit(): void;
    LoadStudyNumber(): void;
    handleUpload(data: any): void;
    goToSearch(): void;
}
