import { OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from './auth.service';
import { loginInfo } from './auth.classes';
export declare class LoginComponent implements OnInit {
    authService: AuthService;
    router: Router;
    message: string;
    logInfo: loginInfo;
    loading: boolean;
    error: string;
    constructor(authService: AuthService, router: Router);
    ngOnInit(): void;
    setMessage(): void;
    login(): void;
    logout(): void;
}
