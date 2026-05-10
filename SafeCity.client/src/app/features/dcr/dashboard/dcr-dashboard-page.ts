import { Component, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { DcrApiService, Crisis, Team, Deployment } from '../../../core/services/api/dcr-api.service';
import { Sidebar } from '../../../layout/sidebar/sidebar';
import { getUserRole, normalizeRole, isTokenValid } from '../../../shared/auth-utils';

@Component({
  selector: 'app-dcr-dashboard-page',
  standalone: true,
  imports: [DatePipe, Sidebar],
  templateUrl: './dcr-dashboard-page.html',
  styleUrl: './dcr-dashboard-page.css',
})
export class DcrDashboardPage implements OnInit {
  canViewCrisis = false;
  canViewTeams = false;
  canViewDeployments = false;
  activeCrises: Crisis[] = [];
  availableTeams: Team[] = [];
  activeDeployments: Deployment[] = [];
  loadingCrises = false;
  loadingTeams = false;
  loadingDeployments = false;
  error = '';

  constructor(private api: DcrApiService) {}

  ngOnInit(): void {
    const role = normalizeRole(getUserRole());
    this.canViewCrisis = ['city_administrator', 'emergency_dispatcher'].includes(role);
    this.canViewTeams = role === 'city_administrator';
    this.canViewDeployments = ['city_administrator', 'emergency_dispatcher', 'police', 'police_officer'].includes(role);
    this.refresh();
  }

  get responseReadyCount(): number {
    return this.activeDeployments.filter(d => d.status === 'Active' || d.status === 'Pending').length;
  }

  refresh(): void {
    if (!isTokenValid()) { this.error = 'Please login again.'; return; }
    this.error = '';
    this.loadCrises();
    this.loadTeams();
    this.loadDeployments();
  }

  private async loadCrises(): Promise<void> {
    if (!this.canViewCrisis) return;
    this.loadingCrises = true;
    try {
      this.activeCrises = await this.api.getActiveCrises();
    } catch { this.error = 'Failed to load crises.'; }
    finally { this.loadingCrises = false; }
  }

  private async loadTeams(): Promise<void> {
    if (!this.canViewTeams) return;
    this.loadingTeams = true;
    try {
      this.availableTeams = await this.api.getAvailableTeams();
    } catch { this.error = 'Failed to load teams.'; }
    finally { this.loadingTeams = false; }
  }

  private async loadDeployments(): Promise<void> {
    if (!this.canViewDeployments) return;
    this.loadingDeployments = true;
    try {
      this.activeDeployments = await this.api.getActiveDeployments();
    } catch { this.error = 'Failed to load deployments.'; }
    finally { this.loadingDeployments = false; }
  }
}
