import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';

interface ProblemDetails {
  title?: string;
  detail?: string;
  errors?: Record<string, string[]>;
}

@Injectable({ providedIn: 'root' })
export class ApiErrorService {
  getMessage(error: unknown, fallback: string): string {
    if (!(error instanceof HttpErrorResponse)) {
      return fallback;
    }

    console.error('API Error:', error);

    let body = error.error as ProblemDetails | null;

    if (typeof error.error === 'string' && error.error) {
      try {
        body = JSON.parse(error.error) as ProblemDetails;
      } catch {
        return error.error;
      }
    }

    if (body?.errors) {
      const messages = Object.values(body.errors).flat();
      if (messages.length > 0) {
        return messages.join(' ');
      }
    }

    if (body?.detail) {
      return body.detail;
    }

    if (body?.title) {
      return body.title;
    }

    if (error.status === 0) {
      return 'Unable to reach the API. Ensure the backend is running on http://localhost:5000.';
    }

    if (error.status === 401 || error.status === 403) {
      return 'You are not authorized to perform this action.';
    }

    if (error.statusText) {
      return error.statusText;
    }

    return fallback;
  }
}
