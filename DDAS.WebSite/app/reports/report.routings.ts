import { ModuleWithProviders }   from '@angular/core';
import { Routes, RouterModule }  from '@angular/router';
import { ReportComponent } from  './report.component';
// import { UserInputComponent} from './user-input.component';
// import { routing,
//          appRoutingProviders } from './app.routing';
import { SearchComponent }     from '../search/search.component';

export const ReportRoutes: Routes = [
  { path: 'report', component: ReportComponent},
  { path: '',redirectTo: '/report', pathMatch: 'full'}
//   // },
// //    children: [
// //   { path: 'User-input', component: UserInputComponent },
// //   ]
// //   },
]

//   // {
//   //   path: '',
//   //   redirectTo: '/User',
//   //   pathMatch: 'full'
//   // },
//   // { path: 'User-input/:id', component: UserInputComponent },
//   // { path: 'search', component: SearchComponent ,
    
//   //   children: [
//   //         { path: 'User-input',  component: UserInputComponent },
//   //     ]   
//   // },



export const ReportRouting: ModuleWithProviders = RouterModule.forChild(ReportRoutes);

