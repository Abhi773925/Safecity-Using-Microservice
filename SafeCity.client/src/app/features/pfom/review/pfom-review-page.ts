import { Component, OnInit } from '@angular/core';
import { DatePipe, NgClass } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PfomApiService, FieldReport } from '../../../core/services/api/pfom-api.service';
import { Sidebar } from '../../../layout/sidebar/sidebar';
import { getUserRole, normalizeRole } from '../../../shared/auth-utils';

@Component({
  selector: 'app-pfom-review-page',
  standalone: true,
  imports: [FormsModule, DatePipe, NgClass, Sidebar],
  templateUrl: './pfom-review-page.html',
  styleUrl: './pfom-review-page.css',
})
export class PfomReviewPage implements OnInit {
  canReview = false;
  reports: FieldReport[] = [];
  loading = false;
  submitting = false;
  message = '';
  messageType: 'success' | 'error' = 'success';
  searchText = '';
  statusFilter = 'All';

  constructor(private api: PfomApiService) {}

  ngOnInit(): void {
    this.canReview = ['emergency_dispatcher', 'city_administrator']
      .includes(normalizeRole(getUserRole()));
    this.loadReports();
  }

  get filteredReports(): FieldReport[] {
    const q = this.searchText.trim().toLowerCase();
    return this.reports.filter(r => {
      const searchMatch = !q ||
        String(r.reportId).includes(q) ||
        String(r.patrolId).includes(q) ||
        r.notes.toLowerCase().includes(q);
      const statusMatch = this.statusFilter === 'All' || r.status === this.statusFilter;
      return searchMatch && statusMatch;
    });
  }

  async loadReports(): Promise<void> {
    if (!this.canReview) { this.reports = []; return; }
    this.loading = true;
    try {
      this.reports = await this.api.getAllFieldReports();
    } catch (err: any) {
      this.reports = [];
      this.showMessage(err?.response?.data?.message || 'Failed to load reports.', 'error');
    } finally {
      this.loading = false;
    }
  }

  async reviewReport(reportId: number, newStatus: number): Promise<void> {
    if (!this.canReview) return;
    this.submitting = true;
    try {
      await this.api.reviewFieldReport(reportId, newStatus);
      this.showMessage('Report status updated.', 'success');
      await this.loadReports();
    } catch (err: any) {
      this.showMessage(err?.response?.data?.message || 'Failed to update report.', 'error');
    } finally {
      this.submitting = false;
    }
  }

  statusClass(status: string): string {
    if (status === 'Approved') return 'border-sky-200 bg-sky-50 text-sky-700';
    if (status === 'Rejected') return 'border-red-200 bg-red-50 text-red-700';
    if (status === 'Closed') return 'border-slate-300 bg-slate-100 text-slate-700';
    return 'border-amber-200 bg-amber-50 text-amber-700';
  }

  private showMessage(msg: string, type: 'success' | 'error'): void {
    this.message = msg;
    this.messageType = type;
  }
}
