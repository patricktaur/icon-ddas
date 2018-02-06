import {Injectable} from '@angular/core';
import { Subject } from 'rxjs/Subject';

@Injectable()
export class StatusActivatorService{
    private reLoadSubject = new Subject<any>();

    reloadEvent(event){
        this.reLoadSubject.next(event);
    }    

    get events$(){
        return this.reLoadSubject.asObservable();
    }
}