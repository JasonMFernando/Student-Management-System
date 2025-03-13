import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { StatusService } from '../services/status.service';

@Component({
  selector: 'app-create-status-dialog',
  templateUrl: './create-status-dialog.component.html',
  styleUrl: './create-status-dialog.component.css'
})
export class CreateStatusDialogComponent {
  statusForm: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<CreateStatusDialogComponent>,
    private fb: FormBuilder,
    private statusService: StatusService
  ) {
    this.statusForm = this.fb.group({
      statusName: ['', Validators.required]
    });
  }

  onSubmit() {
    if (this.statusForm.valid) {
      this.statusService.createStatus(this.statusForm.value).subscribe(
        () => {
          this.dialogRef.close(true);
        },
        (error) => {
          console.error('Failed to create status:', error);
        }
      );
    }
  }

  onCancel() {
    this.dialogRef.close(false);
  }
}
