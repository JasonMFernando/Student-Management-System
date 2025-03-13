import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { RoleService } from '../services/role.service';

@Component({
  selector: 'app-create-role-dialog',
  templateUrl: './create-role-dialog.component.html',
  styleUrl: './create-role-dialog.component.css'
})
export class CreateRoleDialogComponent implements OnInit {
  roleForm!: FormGroup;
  statuses: any[] = [];

  constructor(
    private fb: FormBuilder,
    private roleService: RoleService,
    public dialogRef: MatDialogRef<CreateRoleDialogComponent>
  ) {}

  ngOnInit(): void {
    this.roleForm = this.fb.group({
      roleName: ['', Validators.required],
      status: ['', Validators.required]
    });

    this.loadStatuses();
  }

  loadStatuses(): void {
    this.roleService.getAllStatuses().subscribe(
      (data) => (this.statuses = data),
      (error) => console.error('Error loading statuses', error)
    );
  }

  onSubmit(): void {
    if (this.roleForm.valid) {
      this.roleService.createRole(this.roleForm.value).subscribe(
        () => this.dialogRef.close(true),
        (error) => console.error('Role creation failed', error)
      );
    }
  }
}
