import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthServiceTsService } from '../Services/auth.service.ts.service';

export const adminGuard: CanActivateFn = () => {
  const authService = inject(AuthServiceTsService);
  const router = inject(Router);

  if (authService.isLoggedIn() && authService.getIsAdmin()) {
    return true;
  }

  router.navigate(['/home']);
  return false;
};
