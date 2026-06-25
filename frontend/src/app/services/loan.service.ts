import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  CreateLoanRequest,
  Loan,
  UpdateLoanRequest,
} from '../models/loan.model';

@Injectable({ providedIn: 'root' })
export class LoanService {
  private readonly baseUrl = `${environment.apiUrl}/loans`;

  constructor(private readonly http: HttpClient) {}

  getLoans(): Promise<Loan[]> {
    return firstValueFrom(this.http.get<Loan[]>(this.baseUrl));
  }

  getLoanById(id: string): Promise<Loan> {
    return firstValueFrom(this.http.get<Loan>(`${this.baseUrl}/${id}`));
  }

  createLoan(request: CreateLoanRequest): Promise<string> {
    return firstValueFrom(this.http.post<string>(this.baseUrl, request));
  }

  updateLoan(id: string, request: UpdateLoanRequest): Promise<void> {
    return firstValueFrom(
      this.http.put(`${this.baseUrl}/${id}`, request, { responseType: 'text' })
    ).then(() => undefined);
  }

  deleteLoan(id: string): Promise<void> {
    return firstValueFrom(
      this.http.delete(`${this.baseUrl}/${id}`, { responseType: 'text' })
    ).then(() => undefined);
  }

  registerPayment(id: string, amount: number): Promise<void> {
    return firstValueFrom(
      this.http.post(`${this.baseUrl}/${id}/payment`, { amount }, { responseType: 'text' })
    ).then(() => undefined);
  }
}
