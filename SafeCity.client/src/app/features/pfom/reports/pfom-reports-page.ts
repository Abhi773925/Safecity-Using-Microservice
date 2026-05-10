import { Component, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PfomApiService, FieldReport } from '../../../core/services/api/pfom-api.service';
import { Sidebar } from '../../../layout/sidebar/sidebar';
import { getUserRole, normalizeRole } from '../../../shared/auth-utils';

@Component({
  selector: 'app-pfom-reports-page',
  standalone: true,
  imports: [FormsModule, DatePipe, Sidebar],
  templateUrl: './pfom-reports-page.html',
  styleUrl: './pfom-reports-page.css',
})
export class PfomReportsPage implements OnInit {
  canSubmit = false;
  reports: FieldReport[] = [];
  loadingHistory = false;
  submitting = false;
  message = '';
  messageType: 'success' | 'error' = 'success';

  patrolId = 0;
  reportDate = '';
  notes = '';

  constructor(private api: PfomApiService) {}

  ngOnInit(): void {
    this.canSubmit = ['police', 'police_officer', 'emergency_dispatcher', 'fire_fighter']
      .includes(normalizeRole(getUserRole()));
    this.loadMyHistory();
  }

  async loadMyHistory(): Promise<void> {
    this.loadingHistory = true;
    try {
      this.reports = await this.api.getMyFieldReports();
    } catch (err: any) {
      this.reports = [];
      this.showMessage(err?.response?.data?.message || 'Failed to load history.', 'error');
    } finally {
      this.loadingHistory = false;
    }
  }

  async submitReport(): Promise<void> {
    if (!this.canSubmit) return;
    if (this.patrolId <= 0 || !this.notes.trim() || this.notes.trim().length < 10) {
      this.showMessage('Valid patrol ID and at least 10 characters in notes required.', 'error');
      return;
    }
    this.submitting = true;

    const payload = {
      patrolId: Number(this.patrolId),
      notes: this.notes.trim(),
      date: this.reportDate ? new Date(this.reportDate).toISOString() : new Date().toISOString(),
      status: 1,
    };

    try {
      await this.api.submitFieldReport(payload);
      this.showMessage('Field report submitted successfully.', 'success');
      this.patrolId = 0;
      this.notes = '';
      this.reportDate = '';
      await this.loadMyHistory();
    } catch (err: any) {
      this.showMessage(err?.response?.data?.message || 'Failed to submit report.', 'error');
    } finally {
      this.submitting = false;
    }
  }

  private showMessage(msg: string, type: 'success' | 'error'): void {
    this.message = msg;
    this.messageType = type;
  }
}
