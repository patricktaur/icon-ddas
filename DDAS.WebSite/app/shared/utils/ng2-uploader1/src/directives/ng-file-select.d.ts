import { ElementRef, EventEmitter } from '@angular/core';
import { Ng2Uploader, UploadRejected } from '../services/ng2-uploader';
export declare class NgFileSelectDirective {
    el: ElementRef;
    events: EventEmitter<any>;
    onUpload: EventEmitter<any>;
    onPreviewData: EventEmitter<any>;
    onUploadRejected: EventEmitter<UploadRejected>;
    _options: any;
    options: any;
    files: any[];
    uploader: Ng2Uploader;
    constructor(el: ElementRef);
    filterFilesByExtension(): void;
    onChange(): void;
}
