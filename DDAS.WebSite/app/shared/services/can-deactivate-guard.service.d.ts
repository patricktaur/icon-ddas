/// <reference types="core-js" />
import { CanDeactivate } from '@angular/router';
import { Observable } from 'rxjs/Observable';
export interface CanComponentDeactivate {
    canDeactivate: () => boolean | Observable<boolean>;
}
export declare class CanDeactivateGuard implements CanDeactivate<CanComponentDeactivate> {
    canDeactivate(component: CanComponentDeactivate): Observable<boolean> | Promise<boolean> | boolean;
}
