export type LoanStatus = 'active' | 'paid';

export interface Loan {
  id: string;
  amount: number;
  currentBalance: number;
  applicantName: string;
  status: LoanStatus;
}

export interface CreateLoanRequest {
  amount: number;
  applicantName: string;
}

export interface UpdateLoanRequest {
  amount: number;
  applicantName: string;
  currentBalance: number;
  status: LoanStatus;
}

export interface RegisterPaymentRequest {
  amount: number;
}

export interface LoanTableRow {
  id: string;
  loanAmount: number;
  currentBalance: number;
  applicant: string;
  status: LoanStatus;
}
