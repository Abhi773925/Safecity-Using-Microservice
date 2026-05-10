import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  // Redirect root to role-based dashboard
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () => import('./core/role-redirect').then(m => m.RoleRedirect),
  },

  // Public routes — no auth required
  {
    path: 'hero',
    loadComponent: () => import('./features/hero/hero.component').then(m => m.HeroComponent),
  },
  {
    path: 'login',
    loadComponent: () => import('./features/iam/auth/login/login').then(m => m.Login),
  },
  {
    path: 'register',
    loadComponent: () => import('./features/iam/auth/register/register').then(m => m.Register),
  },

  // Account — auth required
  {
    path: 'change-password',
    canActivate: [authGuard],
    loadComponent: () => import('./features/iam/account/change-password/change-password').then(m => m.ChangePassword),
  },
  {
    path: 'delete-account',
    canActivate: [authGuard],
    loadComponent: () => import('./features/iam/account/delete-account/delete-account').then(m => m.DeleteAccount),
  },

  // IRCM — Incident & Case Management
  {
    path: 'ircm/report',
    canActivate: [authGuard],
    loadComponent: () => import('./features/ircm/report/report-page').then(m => m.ReportPage),
  },
  {
    path: 'ircm/incidents',
    canActivate: [authGuard],
    loadComponent: () => import('./features/ircm/incidents/incidents-page').then(m => m.IncidentsPage),
  },
  {
    path: 'ircm/cases',
    canActivate: [authGuard],
    loadComponent: () => import('./features/ircm/cases/cases-page').then(m => m.CasesPage),
  },

  // EDRA — Emergency Dispatch & Resources
  {
    path: 'edra/dashboard',
    canActivate: [authGuard],
    loadComponent: () => import('./features/edra/dashboard/edra-dashboard-page').then(m => m.EdraDashboardPage),
  },
  {
    path: 'edra/resources',
    canActivate: [authGuard],
    loadComponent: () => import('./features/edra/resources/edra-resources-page').then(m => m.EdraResourcesPage),
  },
  {
    path: 'edra/dispatch',
    canActivate: [authGuard],
    loadComponent: () => import('./features/edra/dispatch/edra-dispatch-page').then(m => m.EdraDispatchPage),
  },

  // PFOM — Patrol & Field Operations
  {
    path: 'pfom/dashboard',
    canActivate: [authGuard],
    loadComponent: () => import('./features/pfom/dashboard/pfom-dashboard-page').then(m => m.PfomDashboardPage),
  },
  {
    path: 'pfom/patrols',
    canActivate: [authGuard],
    loadComponent: () => import('./features/pfom/patrols/pfom-patrols-page').then(m => m.PfomPatrolsPage),
  },
  {
    path: 'pfom/reports',
    canActivate: [authGuard],
    loadComponent: () => import('./features/pfom/reports/pfom-reports-page').then(m => m.PfomReportsPage),
  },
  {
    path: 'pfom/review',
    canActivate: [authGuard],
    loadComponent: () => import('./features/pfom/review/pfom-review-page').then(m => m.PfomReviewPage),
  },

  // DCR — Disaster & Crisis Response
  {
    path: 'dcr/dashboard',
    canActivate: [authGuard],
    loadComponent: () => import('./features/dcr/dashboard/dcr-dashboard-page').then(m => m.DcrDashboardPage),
  },
  {
    path: 'dcr/crisis',
    canActivate: [authGuard],
    loadComponent: () => import('./features/dcr/crisis/dcr-crisis-page').then(m => m.DcrCrisisPage),
  },
  {
    path: 'dcr/team',
    canActivate: [authGuard],
    loadComponent: () => import('./features/dcr/team/dcr-team-page').then(m => m.DcrTeamPage),
  },
  {
    path: 'dcr/response',
    canActivate: [authGuard],
    loadComponent: () => import('./features/dcr/response/dcr-response-page').then(m => m.DcrResponsePage),
  },
];
