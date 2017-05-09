import { ModuleWithProviders }   from '@angular/core';
import { Routes, RouterModule }  from '@angular/router';
import { UserComponent } from  './user.component';
import { UserInputComponent} from './user-input.component';
// import { routing,
//          appRoutingProviders } from './app.routing';
import { SearchComponent }     from '../search/search.component';
import { AuthGuard } from '../auth/auth-guard.service';

export const UserRoutes: Routes = [
  { path: 'users', component: UserComponent, canActivate: [AuthGuard]},
  { path: 'user-input/:userid', component: UserInputComponent, canActivate: [AuthGuard]},
  { path: '',redirectTo: '/users', pathMatch: 'full'}
]


export const UserRouting: ModuleWithProviders = RouterModule.forChild(UserRoutes);

