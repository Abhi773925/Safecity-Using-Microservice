import { Component, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { PfomApiService, Patrol, FieldReport } from '../../../core/services/api/pfom-api.service';
import { Sidebar } from '../../../layout/sidebar/sidebar';
import { getUserRole, normalizeRole } from '../../../shared/auth-utils';

@Component({
  selector: 'app-pfom-dashboard-page',
  standalone: true,
  imports: [DatePipe, Sidebar],
  templateUrl: './pfom-dashboard-page.html',
  styleUrl: './pfom-dashboard-page.css',
})
export class PfomDashboardPage implements OnInit {
  canOperate = false;
  canSubmit = false;
  canReview = false;
  patrols: Patrol[] = [];
  reports: FieldReport[] = [];
  loadingPatrols = false;
  loadingReports = false;
  error = '';
  reviewPendingCount = 0;
  get myReports() { return this.reports; }
  get dashboardReports() { return this.reports; }

  constructor(private api: PfomApiService) { }

  ngOnInit(): void {
    const role = normalizeRole(getUserRole());
    this.canOperate = ['police', 'police_officer', 'emergency_dispatcher', 'fire_fighter'].includes(role);
    this.canSubmit = ['police', 'police_officer', 'emergency_dispatcher', 'fire_fighter'].includes(role);
    this.canReview = ['emergency_dispatcher', 'city_administrator'].includes(role);
    this.refresh();
  }

  get onPatrolCount(): number {
    return this.patrols.filter(p => p.status === 'OnPatrol').length;
  }

  get pendingReviewCount(): number {
    return this.reports.filter(r => ['Submitted', 'InReview', 'Draft'].includes(r.status)).length;
  }

  refresh(): void {
    this.error = '';
    this.loadPatrols();
    this.loadReports();
  }

  private async loadPatrols(): Promise<void> {
    if (!this.canOperate) { this.patrols = []; return; }
    this.loadingPatrols = true;
    try {
      this.patrols = await this.api.getMyPatrols();
    } catch (err: any) {
      this.error = err?.response?.data?.message || 'Failed to load patrols.';
      this.patrols = [];
    } finally {
      this.loadingPatrols = false;
    }
  }

  private async loadReports(): Promise<void> {
    this.loadingReports = true;
    try {
      if (this.canReview) {
        this.reports = await this.api.getAllFieldReports();
      } else if (this.canSubmit) {
        this.reports = await this.api.getMyFieldReports();
      } else {
        this.reports = [];
      }
    } catch (err: any) {
      this.error = err?.response?.data?.message || 'Failed to load reports.';
      this.reports = [];
    } finally {
      this.loadingReports = false;
    }
  }
}
