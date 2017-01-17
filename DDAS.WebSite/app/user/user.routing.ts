import { ModuleWithProviders }   from '@angular/core';
import { Routes, RouterModule }  from '@angular/router';
import { UserComponent } from  './user.component';
import { UserInputComponent} from './user-input.component';
// import { routing,
//          appRoutingProviders } from './app.routing';
import { SearchComponent }     from '../search/search.component';

export const UserRoutes: Routes = [
  { path: 'users', component: UserComponent},
  { path: 'user-input/:userid', component: UserInputComponent},
  { path: '',redirectTo: '/users', pathMatch: 'full'},

]


export const UserRouting: ModuleWithProviders = RouterModule.forChild(UserRoutes);

