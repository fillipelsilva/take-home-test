import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import {
  MAT_DIALOG_DATA,
  MatDialogModule,
  MatDialogRef,
} from '@angular/material/dialog';

export interface DeleteLoanDialogData {
  applicantName: string;
}

@Component({
  selector: 'app-delete-loan-dialog',
  standalone: true,
  imports: [MatDialogModule, MatButtonModule],
  template: `
    <h2 mat-dialog-title>Delete Loan</h2>
    <mat-dialog-content>
      Are you sure you want to delete the loan for
      <strong>{{ data.applicantName }}</strong>?
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button type="button" (click)="cancel()">Cancel</button>
      <button mat-flat-button color="warn" type="button" (click)="confirm()">
        Confirm
      </button>
    </mat-dialog-actions>
  `,
})
export class DeleteLoanDialogComponent {
  readonly data = inject<DeleteLoanDialogData>(MAT_DIALOG_DATA);
  private readonly dialogRef = inject(MatDialogRef<DeleteLoanDialogComponent>);

  cancel(): void {
    this.dialogRef.close(false);
  }

  confirm(): void {
    this.dialogRef.close(true);
  }
}
