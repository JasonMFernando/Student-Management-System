import { Component, Inject , OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { UserService } from '../user.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-edit-user-dialog',
  templateUrl: './edit-user-dialog.component.html',
  styleUrl: './edit-user-dialog.component.css'
})
export class EditUserDialogComponent implements OnInit {
  userForm!: FormGroup;
  roles: any[] = [];
  statuses: any[] = [];

  constructor(
    public dialogRef: MatDialogRef<EditUserDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { userID: number },
    private fb: FormBuilder,
    private userService: UserService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit() {
    this.userForm = this.fb.group({
      firstName: [''],
      lastName: [''],
      email: [''],
      password: [''],
      dateOfBirth: [''],
      roleType: [''],
      status: ['']
    });

    // Fetch user data by ID
    this.userService.getUserById(this.data.userID).subscribe((userData) => {
      this.userForm.patchValue({
        firstName: userData.firstName,
        lastName: userData.lastName,
        email: userData.email,
        password: '', 
        dateOfBirth: userData.dateOfBirth.split('T')[0], 
        roleType: userData.role.roleID,
        status: userData.status.statusID
      });
    });

    // Load Roles
    this.userService.getAllRoles().subscribe((roles) => {
      this.roles = roles;
    });

    // Load Statuses
    this.userService.getAllStatuses().subscribe((statuses) => {
      this.statuses = statuses;
    });
  }

  onSubmit() {
    if (this.userForm.valid) {
      const updatedUser = {
        userID: this.data.userID,
        ...this.userForm.value
      };

      this.userService.updateUser(updatedUser).subscribe(
        (response) => {
          console.log('User Updated:', response);
          this.snackBar.open('Student Updated successfully!', 'Close', {
            duration: 3000,
            panelClass: ['snackbar-success']
          });
          this.dialogRef.close(true); 
        },
        (error) => {
          console.error('Update failed:', error);
          this.snackBar.open('Error Updating Student. Please try again.', 'Close', {
            duration: 3000,
            panelClass: ['snackbar-error']
          });
        }
      );
    }
  }

  onCancel() {
    this.dialogRef.close(false);
  }
}
