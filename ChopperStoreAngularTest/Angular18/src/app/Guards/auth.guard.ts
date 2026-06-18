import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthServiceTsService } from '../Services/auth.service.ts.service';

export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthServiceTsService);
  const router = inject(Router);

  if (authService.isLoggedIn()) {
    return true;
  }

  router.navigate(['/login']);
  return false;
};
