import { ModuleWithProviders }  from '@angular/core';

import { Routes, RouterModule } from '@angular/router';

import { SearchComponent } from './search.component';
import { AuthGuard }             from '../auth/auth-guard.service';
import { SearchReport } from './search-report.component';
import {PageNotFoundComponent} from '../page-not-found.component';

export const searchRoutes: Routes = [
     { path: 'search', component: SearchComponent , canActivate: [AuthGuard] },
     { path: '', redirectTo:'search',  pathMatch: 'full' },
      { path: 'search-report', component: SearchReport , canActivate: [AuthGuard] },
      { path: '**',     component: PageNotFoundComponent    },
];


export const searchRouting: ModuleWithProviders = RouterModule.forChild(searchRoutes);