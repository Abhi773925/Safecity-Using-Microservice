import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { isTokenValid } from '../../shared/auth-utils';

// Blocks access to protected pages if user is not logged in
export const authGuard: CanActivateFn = () => {
  const router = inject(Router);
  if (isTokenValid()) return true;
  router.navigate(['/login']);
  return false;
};
