import { ModuleWithProviders }  from '@angular/core';

import { Routes, RouterModule } from '@angular/router';

import { SearchComponent } from './search.component';
import { AuthGuard }             from '../auth/auth-guard.service';

export const searchRoutes: Routes = [
     { path: 'search', component: SearchComponent , canActivate: [AuthGuard] },
     { path: '', redirectTo:'search',  pathMatch: 'full' },
];


export const searchRouting: ModuleWithProviders = RouterModule.forChild(searchRoutes);