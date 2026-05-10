import { Component, OnInit } from '@angular/core';
import { DatePipe, NgClass } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PfomApiService, Patrol } from '../../../core/services/api/pfom-api.service';
import { Sidebar } from '../../../layout/sidebar/sidebar';
import { getUserRole, normalizeRole } from '../../../shared/auth-utils';

@Component({
  selector: 'app-pfom-patrols-page',
  standalone: true,
  imports: [FormsModule, DatePipe, NgClass, Sidebar],
  templateUrl: './pfom-patrols-page.html',
  styleUrl: './pfom-patrols-page.css',
})
export class PfomPatrolsPage implements OnInit {
  canSchedule = false;
  canOperate = false;
  patrols: Patrol[] = [];
  patrolHistoryList: Patrol[] = [];
  loadingPatrols = false;
  submitting = false;
  submittingStatus = false;
  message = '';
  messageType: 'success' | 'error' = 'success';
  patrolHistory() { return this.patrolHistoryList; }
  setPatrolHistory(value: Patrol[]) { this.patrolHistoryList = value; }

  officerId = 0;
  area = '';
  patrolDate = '';
  patrolStatus = 'Inactive';

  private readonly statusMap: Record<string, number> = { Active: 0, OnPatrol: 1, Inactive: 2 };

  constructor(private api: PfomApiService) { }

  ngOnInit(): void {
    const role = normalizeRole(getUserRole());
    this.canSchedule = ['emergency_dispatcher', 'city_administrator', 'compliance_officer'].includes(role);
    this.canOperate = ['police', 'police_officer', 'emergency_dispatcher', 'fire_fighter'].includes(role);
    this.loadPatrols();
    this.loadHistory();
  }

  get onPatrolCount(): number {
    return this.patrols.filter(p => p.status === 'OnPatrol').length;
  }

  async loadPatrols(): Promise<void> {
    if (!this.canOperate) { this.patrols = []; return; }
    this.loadingPatrols = true;
    try {
      this.patrols = await this.api.getMyPatrols();
    } catch (err: any) {
      this.showMessage(err?.response?.data?.message || 'Failed to load patrols.', 'error');
    } finally {
      this.loadingPatrols = false;
    }
  }

  async loadHistory(): Promise<void> {
    if (!this.canOperate) { this.setPatrolHistory([]); return; }
    try {
      this.setPatrolHistory(await this.api.getPatrolHistory());
    } catch (err: any) {
      this.showMessage(err?.response?.data?.message || 'Failed to load history.', 'error');
    }
  }

  async schedulePatrol(): Promise<void> {
    if (!this.canSchedule) return;
    if (this.officerId <= 0 || !this.area.trim() || !this.patrolDate) {
      this.showMessage('Officer ID, area, and date are required.', 'error');
      return;
    }
    this.submitting = true;

    const payload = {
      officerId: Number(this.officerId),
      area: this.area.trim(),
      date: new Date(this.patrolDate).toISOString(),
      status: this.statusMap[this.patrolStatus] ?? 2,
    };

    try {
      await this.api.schedulePatrol(payload);
      this.showMessage('Patrol scheduled successfully.', 'success');
      this.officerId = 0;
      this.area = '';
      this.patrolDate = '';
      this.patrolStatus = 'Inactive';
      await this.loadPatrols();
      await this.loadHistory();
    } catch (err: any) {
      this.showMessage(err?.response?.data?.message || 'Failed to schedule patrol.', 'error');
    } finally {
      this.submitting = false;
    }
  }

  async updateStatus(patrolId: number, status: 'OnPatrol' | 'Inactive'): Promise<void> {
    if (!this.canOperate) return;
    this.submittingStatus = true;

    const payload = { patrolId, newStatus: this.statusMap[status] };

    try {
      await this.api.updatePatrolStatus(payload);
      this.showMessage(`Status updated to ${status}.`, 'success');
      await this.loadPatrols();
      await this.loadHistory();
    } catch (err: any) {
      this.showMessage(err?.response?.data?.message || 'Failed to update status.', 'error');
    } finally {
      this.submittingStatus = false;
    }
  }

  private showMessage(msg: string, type: 'success' | 'error'): void {
    this.message = msg;
    this.messageType = type;
  }
}
