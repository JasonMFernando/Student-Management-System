import { Component, OnInit, EventEmitter, Output } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-create-user-dialog',
  templateUrl: './create-user-dialog.component.html',
  styleUrl: './create-user-dialog.component.css'
})
export class CreateUserDialogComponent {
  @Output() userCreated: EventEmitter<void> = new EventEmitter<void>();
  userForm: FormGroup;
  statuses: any[] = [];
  roles: any[] = [];
  token: string | null = localStorage.getItem('token'); 

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<CreateUserDialogComponent>,
    private http: HttpClient,
    private snackBar: MatSnackBar
  ) {
    this.userForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      roleType: ['', Validators.required],
      status: ['', Validators.required]
    });
  }

  ngOnInit(){
    this.getStatuses();
    this.getRoles();
  }

  getStatuses() {
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.token}`
    });

    this.http.get<any[]>('https://localhost:44390/api/Status/GetAllStatuses', { headers }).subscribe({
      next: (data) => {
        this.statuses = data;
      },
      error: (err) => {
        console.error('Error fetching statuses:', err);
      }
    });
  }

  getRoles() {
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.token}`
    });

    this.http.get<any[]>('https://localhost:44390/api/Role/GetAllRoles', { headers }).subscribe({
      next: (data) => {
        this.roles = data;
      },
      error: (err) => {
        console.error('Error fetching roles:', err);
      }
    });
  }

  onClose() {
    this.dialogRef.close();
  }

  onSubmit() {
    if (this.userForm.valid) {
      const selectedStatusID = this.userForm.value.status;
      const selectedRoleID = this.userForm.value.roleType;
      const dateOfBirth = new Date(this.userForm.value.dateOfBirth).toISOString().split('T')[0];

      const newUser = {
        firstName: this.userForm.value.firstName,
        lastName: this.userForm.value.lastName,
        email: this.userForm.value.email,
        password: this.userForm.value.password,
        dateOfBirth: dateOfBirth,
        status: selectedStatusID,
        roleType: selectedRoleID
      };

      console.log('User Data:', newUser);
      const headers = new HttpHeaders({
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      });


      this.http.post('https://localhost:44390/api/User/CreateUser', newUser, { headers }).subscribe({
        next: (response) => {
          console.log('User created successfully:', response);
          this.snackBar.open('Student created successfully!', 'Close', {
            duration: 3000,
            panelClass: ['snackbar-success']
          });
          this.userCreated.emit();
          this.dialogRef.close(response);
        },
        error: (error) => {
          console.error('Error creating user:', error);
          this.snackBar.open('Error creating Student. Please try again.', 'Close', {
            duration: 3000,
            panelClass: ['snackbar-error']
          });
        }
      });
      // this.dialogRef.close(this.userForm.value);
    }
  }
}
