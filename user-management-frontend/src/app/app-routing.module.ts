import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { UserManagementComponent } from './user-management/user-management.component';

const routes: Routes = [
  {path: '', component: LoginComponent},
  { path: 'user-management', component: UserManagementComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
