import { Router } from '@angular/router';
import { AuthService } from './auth/auth.service';
export declare class AppComponent {
    authService: AuthService;
    private router;
    constructor(authService: AuthService, router: Router);
    logout(): void;
}
