import { Component, OnInit, ViewEncapsulation, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import {
  DeleteLoanDialogComponent,
  DeleteLoanDialogData,
} from '../../components/delete-loan-dialog/delete-loan-dialog.component';
import { Loan, LoanStatus, LoanTableRow } from '../../models/loan.model';
import { ApiErrorService } from '../../services/api-error.service';
import { LoanService } from '../../services/loan.service';

@Component({
  selector: 'app-loans',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDialogModule,
  ],
  templateUrl: './loans.component.html',
  styleUrl: './loans.component.scss',
  encapsulation: ViewEncapsulation.None,
})
export class LoansComponent implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly loanService = inject(LoanService);
  private readonly apiErrorService = inject(ApiErrorService);
  private readonly dialog = inject(MatDialog);

  displayedColumns = [
    'loanAmount',
    'currentBalance',
    'applicant',
    'status',
    'actions',
  ];

  readonly statusOptions: LoanStatus[] = ['active', 'paid'];

  loans: LoanTableRow[] = [];
  errorMessage = '';
  successMessage = '';
  isFormVisible = false;
  isSubmitting = false;
  editingLoanId: string | null = null;

  readonly loanForm = this.formBuilder.group({
    applicantName: ['', [Validators.required, Validators.maxLength(200)]],
    amount: [null as number | null, [Validators.required, Validators.min(0.01)]],
    currentBalance: [
      null as number | null,
      [Validators.required, Validators.min(0)],
    ],
    status: ['active' as LoanStatus, Validators.required],
  });

  ngOnInit(): void {
    void this.loadLoans();
  }

  get isEditMode(): boolean {
    return this.editingLoanId !== null;
  }

  get formTitle(): string {
    return this.isEditMode ? 'Edit Loan' : 'Add Loan';
  }

  get submitLabel(): string {
    return this.isEditMode ? 'Update Loan' : 'Create Loan';
  }

  async loadLoans(): Promise<void> {
    this.clearMessages();

    try {
      const loans = await this.loanService.getLoans();
      this.loans = loans.map((loan) => this.toTableRow(loan));
    } catch (error) {
      console.error('Failed to load loans', error);
      this.errorMessage = this.apiErrorService.getMessage(
        error,
        'Unable to load loans. Ensure the backend is running on http://localhost:5000.'
      );
    }
  }

  toggleForm(): void {
    if (this.isFormVisible && !this.isEditMode) {
      this.hideForm();
      return;
    }

    this.isFormVisible = true;
    this.editingLoanId = null;
    this.resetFormForCreate();
    this.clearMessages();
  }

  startEdit(loan: LoanTableRow): void {
    this.isFormVisible = true;
    this.editingLoanId = loan.id;
    this.clearMessages();

    this.loanForm.reset({
      applicantName: loan.applicant,
      amount: loan.loanAmount,
      currentBalance: loan.currentBalance,
      status: loan.status,
    });
  }

  hideForm(): void {
    this.isFormVisible = false;
    this.editingLoanId = null;
    this.loanForm.reset();
  }

  onAmountChange(): void {
    if (this.isEditMode) {
      return;
    }

    const amount = this.loanForm.controls.amount.value;
    if (amount !== null && amount >= 0) {
      this.loanForm.controls.currentBalance.setValue(amount);
      this.loanForm.controls.status.setValue('active');
    }
  }

  async submitForm(): Promise<void> {
    if (this.loanForm.invalid) {
      this.loanForm.markAllAsTouched();
      return;
    }

    const { applicantName, amount, currentBalance, status } =
      this.loanForm.getRawValue();

    this.isSubmitting = true;
    this.clearMessages();

    try {
      if (this.isEditMode) {
        await this.loanService.updateLoan(this.editingLoanId!, {
          applicantName: applicantName!.trim(),
          amount: amount!,
          currentBalance: currentBalance!,
          status: status!,
        });
        this.successMessage = 'Loan updated successfully.';
      } else {
        await this.loanService.createLoan({
          applicantName: applicantName!.trim(),
          amount: amount!,
        });
        this.successMessage = 'Loan created successfully.';
      }

      this.hideForm();
      await this.loadLoans();
    } catch (error) {
      this.errorMessage = this.apiErrorService.getMessage(
        error,
        this.isEditMode ? 'Unable to update loan.' : 'Unable to create loan.'
      );
    } finally {
      this.isSubmitting = false;
    }
  }

  openDeleteDialog(loan: LoanTableRow): void {
    const dialogRef = this.dialog.open(DeleteLoanDialogComponent, {
      width: '420px',
      data: {
        applicantName: loan.applicant,
      } satisfies DeleteLoanDialogData,
    });

    dialogRef.afterClosed().subscribe((confirmed: boolean) => {
      if (confirmed) {
        void this.deleteLoan(loan);
      }
    });
  }

  private async deleteLoan(loan: LoanTableRow): Promise<void> {
    this.clearMessages();

    try {
      await this.loanService.deleteLoan(loan.id);
      this.successMessage = 'Loan deleted successfully.';

      if (this.editingLoanId === loan.id) {
        this.hideForm();
      }

      await this.loadLoans();
    } catch (error) {
      this.errorMessage = this.apiErrorService.getMessage(
        error,
        'Unable to delete loan.'
      );
    }
  }

  async registerPayment(loan: LoanTableRow): Promise<void> {
    this.clearMessages();

    const input = window.prompt(
      `Enter payment amount for ${loan.applicant} (current balance: ${loan.currentBalance.toFixed(2)})`
    );
    if (!input) {
      return;
    }

    const amount = parseFloat(input);
    if (isNaN(amount) || amount <= 0) {
      this.errorMessage = 'Invalid payment amount.';
      return;
    }

    if (amount > loan.currentBalance) {
      this.errorMessage = 'Payment amount cannot exceed current balance.';
      return;
    }

    this.isSubmitting = true;
    try {
      await this.loanService.registerPayment(loan.id, amount);
      this.successMessage = 'Payment registered successfully.';
      await this.loadLoans();
    } catch (error) {
      this.errorMessage = this.apiErrorService.getMessage(
        error,
        'Unable to register payment.'
      );
    } finally {
      this.isSubmitting = false;
    }
  }

  private resetFormForCreate(): void {
    this.loanForm.reset({
      applicantName: '',
      amount: null,
      currentBalance: null,
      status: 'active',
    });
  }

  private toTableRow(loan: Loan): LoanTableRow {
    return {
      id: loan.id,
      loanAmount: loan.amount,
      currentBalance: loan.currentBalance,
      applicant: loan.applicantName,
      status: loan.status,
    };
  }

  private clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }
}
