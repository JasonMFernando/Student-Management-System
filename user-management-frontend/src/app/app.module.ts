import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { LoginComponent } from './login/login.component';
import { UserManagementComponent } from './user-management/user-management.component';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatDialogModule } from '@angular/material/dialog';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';

import { UserService } from './user.service';
import { CreateUserDialogComponent } from './create-user-dialog/create-user-dialog.component';
import { MatSelectModule } from '@angular/material/select';
import { EditUserDialogComponent } from './edit-user-dialog/edit-user-dialog.component';
import { DeleteUserDialogComponent } from './delete-user-dialog/delete-user-dialog.component';
import { CreateStatusDialogComponent } from './create-status-dialog/create-status-dialog.component';
import { StatusService } from './services/status.service';
import { RoleService } from './services/role.service';
import { CreateRoleDialogComponent } from './create-role-dialog/create-role-dialog.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    UserManagementComponent,
    CreateUserDialogComponent,
    EditUserDialogComponent,
    DeleteUserDialogComponent,
    CreateStatusDialogComponent,
    CreateRoleDialogComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    MatTableModule,
    MatButtonModule,
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    MatDialogModule,
    MatInputModule,
    MatFormFieldModule,
    MatSelectModule
  ],
  providers: [
    provideAnimationsAsync(),
    UserService,
    StatusService,
    RoleService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
