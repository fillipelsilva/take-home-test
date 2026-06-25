import { Routes } from '@angular/router';
import { LoansComponent } from './pages/loans/loans.component';

export const routes: Routes = [
  {
    path: '',
    component: LoansComponent,
  },
  {
    path: '**',
    redirectTo: '',
  },
];
