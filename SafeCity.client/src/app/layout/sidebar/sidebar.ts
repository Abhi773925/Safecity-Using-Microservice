import { Component, OnInit } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

import { getUserRole, normalizeRole } from '../../shared/auth-utils';

interface NavLink {
  label: string;
  path: string;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.css',
})
export class Sidebar implements OnInit {
  role = '';
  moduleLinks: NavLink[] = [];
  crossLinks: NavLink[] = [];

  ngOnInit(): void {
    this.role = normalizeRole(getUserRole());
    this.buildLinks();
  }

  private buildLinks(): void {
    const r = this.role;

    // ── IRCM links ──────────────────────────────────────────────────
    if (['citizen', 'emergency_dispatcher', 'city_administrator', 'fire_fighter'].includes(r)) {
      this.moduleLinks.push({ label: 'Report Incident', path: '/ircm/report' });
    }
    if (['citizen', 'police', 'police_officer', 'city_administrator', 'compliance_officer', 'fire_fighter'].includes(r)) {
      this.moduleLinks.push({ label: r === 'citizen' ? 'My Cases' : 'Case Management', path: '/ircm/cases' });
    }
    if (['citizen', 'police', 'police_officer', 'fire_fighter', 'city_administrator'].includes(r)) {
      this.moduleLinks.push({ label: r === 'citizen' ? 'Queue Status' : 'Incident Queue', path: '/ircm/incidents' });
    }

    // ── PFOM links ──────────────────────────────────────────────────
    if (['police', 'police_officer', 'emergency_dispatcher', 'city_administrator', 'compliance_officer', 'fire_fighter'].includes(r)) {
      this.moduleLinks.push({ label: 'PFOM Dashboard', path: '/pfom/dashboard' });
      this.moduleLinks.push({ label: 'Patrol Ops', path: '/pfom/patrols' });
    }
    if (['police', 'police_officer', 'emergency_dispatcher', 'city_administrator', 'fire_fighter'].includes(r)) {
      this.moduleLinks.push({ label: 'Field Reports', path: '/pfom/reports' });
    }
    if (['emergency_dispatcher', 'city_administrator'].includes(r)) {
      this.moduleLinks.push({ label: 'Review Center', path: '/pfom/review' });
    }

    // ── EDRA links ──────────────────────────────────────────────────
    if (['emergency_dispatcher', 'city_administrator', 'fire_fighter'].includes(r)) {
      this.moduleLinks.push({ label: 'EDRA Dashboard', path: '/edra/dashboard' });
      this.moduleLinks.push({ label: 'Resources', path: '/edra/resources' });
    }
    if (['emergency_dispatcher', 'city_administrator'].includes(r)) {
      this.moduleLinks.push({ label: 'Dispatch Center', path: '/edra/dispatch' });
    }

    // ── DCR links ──────────────────────────────────────────────────
    if (['city_administrator', 'emergency_dispatcher', 'police', 'police_officer'].includes(r)) {
      this.moduleLinks.push({ label: 'Crisis Dashboard', path: '/dcr/dashboard' });
      this.moduleLinks.push({ label: 'Response Ops', path: '/dcr/response' });
    }
    if (['city_administrator', 'emergency_dispatcher'].includes(r)) {
      this.moduleLinks.push({ label: 'Crisis Management', path: '/dcr/crisis' });
      this.moduleLinks.push({ label: 'Team Management', path: '/dcr/team' });
    }

    // ── Account links (always shown when logged in) ─────────────────
    this.crossLinks.push({ label: 'Change Password', path: '/change-password' });
    this.crossLinks.push({ label: 'Delete Account', path: '/delete-account' });
  }
}
