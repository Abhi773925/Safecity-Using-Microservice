import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { getUserRole, normalizeRole, isTokenValid } from '../shared/auth-utils';

@Component({ selector: 'app-role-redirect', template: '', standalone: true })
export class RoleRedirect {
  constructor(router: Router) {
    if (!isTokenValid()) {
      router.navigate(['/hero']);
      return;
    }

    const role = normalizeRole(getUserRole());

    if (['emergency_dispatcher', 'city_administrator'].includes(role)) {
      router.navigate(['/edra/dashboard']);
    } else if (['police', 'police_officer', 'compliance_officer', 'fire_fighter'].includes(role)) {
      router.navigate(['/pfom/dashboard']);
    } else {
      router.navigate(['/ircm/report']);
    }
  }
}
