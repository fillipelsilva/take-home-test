import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';

interface TokenResponse {
  accessToken: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private token: string | null = null;

  constructor(private readonly http: HttpClient) {}

  async getToken(): Promise<string> {
    if (this.token) {
      return this.token;
    }

    const response = await firstValueFrom(
      this.http.post<TokenResponse>(`${environment.apiUrl}/auth/token`, {
        username: 'frontend-user',
        roles: ['LoanAdmin'],
      })
    );

    this.token = response.accessToken;
    return this.token;
  }
}
