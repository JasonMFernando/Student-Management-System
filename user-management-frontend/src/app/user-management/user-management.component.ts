import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { MatTableDataSource } from '@angular/material/table';
import { UserService, User } from '../user.service';
import { MatDialog } from '@angular/material/dialog';
import { CreateUserDialogComponent } from '../create-user-dialog/create-user-dialog.component';
import { EditUserDialogComponent } from '../edit-user-dialog/edit-user-dialog.component';
import { DeleteUserDialogComponent } from '../delete-user-dialog/delete-user-dialog.component';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CreateStatusDialogComponent } from '../create-status-dialog/create-status-dialog.component';
import { CreateRoleDialogComponent } from '../create-role-dialog/create-role-dialog.component';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrl: './user-management.component.css'
})
export class UserManagementComponent implements OnInit {
  users: any[] = [];
  token: string = ''; 
  displayedColumns: string[] = ['userID', 'firstName', 'lastName', 'email', 'dateOfBirth', 'roleName', 'statusName', 'actions']; // Define the columns to display in the table
  dataSource: MatTableDataSource<any> = new MatTableDataSource();
  searchTerm: string = '';
  filteredUsers: any[] = [];

  constructor(private userService: UserService, private dialog: MatDialog, private snackBar: MatSnackBar) { }

  ngOnInit(): void {
    // this.filteredUsers = this.users;
    this.fetchUsers();
  }

  fetchUsers(): void {
    this.userService.getAllUsers().subscribe((users: User[]) => {
      this.dataSource.data = users; 
      this.users = users;
      this.filteredUsers = [...this.users];
    });
  }

  applyFilter() {
    this.filteredUsers = this.users.filter(user => 
      user.firstName.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
      user.lastName.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }

  openEditUserDialog(userID: number) {
    const dialogRef = this.dialog.open(EditUserDialogComponent, {
      width: '400px',
      data: { userID: userID } 
    });
  
    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.fetchUsers();
      }
    });
  }
  

  deleteUser(userID: number) {
    const dialogRef = this.dialog.open(DeleteUserDialogComponent, {
      width: '350px',
      data: { userID }
    });
  
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.userService.deleteUser(userID).subscribe(
          () => {
            console.log('User Deleted Successfully');
            this.snackBar.open('Student Deleted successfully!', 'Close', {
              duration: 3000,
              panelClass: ['snackbar-success']
            });
            this.fetchUsers(); 
          },
          (error) => {
            console.error('Delete failed:', error);
            this.snackBar.open('Error deleting Student. Please try again.', 'Close', {
              duration: 3000,
              panelClass: ['snackbar-error']
            });
          }
        );
      }
    })
  }

  createUser() {
    const dialogRef = this.dialog.open(CreateUserDialogComponent, {
      width: '400px'
    });
  
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        console.log('New User:', result);
        this.fetchUsers();
      }
    });
  }

  openCreateStatusDialog() {
    const dialogRef = this.dialog.open(CreateStatusDialogComponent, {
      width: '350px'
    });
  
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        console.log('Status created successfully');
      }
    });
  }
  
  openCreateRoleDialog(): void {
    const dialogRef = this.dialog.open(CreateRoleDialogComponent, {
      width: '400px'
    });
  
    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        console.log('Role created successfully');
      }
    });
  }
}
