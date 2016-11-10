import { Routes } from '@angular/router';
import { AuthGuard } from './auth-guard.service';
import { AuthService } from './auth.service';
export declare const loginRoutes: Routes;
export declare const authProviders: (typeof AuthService | typeof AuthGuard)[];
