import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/of';
import 'rxjs/add/operator/do';
import 'rxjs/add/operator/delay';
import { Http } from '@angular/http';
import { ConfigService } from '../shared/utils/config.service';
export declare class AuthService {
    private http;
    private configService;
    isLoggedIn: boolean;
    token: string;
    redirectUrl: string;
    _baseUrl: string;
    constructor(http: Http, configService: ConfigService);
    login(username: string, password: string): Observable<boolean>;
    logout(): void;
}
