import { Component, OnInit } from '@angular/core';
import { DatePipe, NgClass, NgForOf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DcrApiService, Crisis } from '../../../core/services/api/dcr-api.service';
import { Sidebar } from '../../../layout/sidebar/sidebar';
import { getUserRole, normalizeRole, isTokenValid } from '../../../shared/auth-utils';

@Component({
  selector: 'app-dcr-crisis-page',
  standalone: true,
  imports: [FormsModule, NgClass, NgForOf, Sidebar],
  templateUrl: './dcr-crisis-page.html',
  styleUrl: './dcr-crisis-page.css',
})
export class DcrCrisisPage implements OnInit {
  canView = false;
  canCreateOrUpdate = false;
  crises: Crisis[] = [];
  loading = false;
  submitting = false;
  message = '';
  messageType: 'success' | 'error' = 'success';
  selectedCrisisId: number | null = null;

  searchText = '';
  statusFilter = 'All';
  type = 0;
  location = '';
  severity = 0;
  status = 0;

  readonly typeOptions = [{ value: 0, label: 'Flood' }, { value: 1, label: 'Earthquake' }, { value: 2, label: 'Fire' }];
  readonly severityOptions = [{ value: 0, label: 'Low' }, { value: 1, label: 'Medium' }, { value: 2, label: 'High' }];
  readonly crisisStatusOptions = ['Pending', 'Active', 'Stabilized', 'Resolved', 'Closed', 'Cancelled'];

  constructor(private api: DcrApiService) { }

  ngOnInit(): void {
    const role = normalizeRole(getUserRole());
    this.canView = ['city_administrator', 'emergency_dispatcher'].includes(role);
    this.canCreateOrUpdate = ['city_administrator', 'emergency_dispatcher'].includes(role);
    this.refresh();
  }

  get filteredCrises(): Crisis[] {
    const q = this.searchText.trim().toLowerCase();
    return this.crises.filter(c => {
      const searchMatch = !q || String(c.crisisId).includes(q) || c.type.toLowerCase().includes(q) ||
        c.location.toLowerCase().includes(q) || c.status.toLowerCase().includes(q);
      const statusMatch = this.statusFilter === 'All' || c.status === this.statusFilter;
      return searchMatch && statusMatch;
    });
  }

  async refresh(): Promise<void> {
    if (!isTokenValid()) { this.showMessage('Please login again.', 'error'); return; }
    if (!this.canView) { this.crises = []; return; }
    this.loading = true;
    try {
      this.crises = await this.api.getActiveCrises();
    } catch (err: any) {
      this.showMessage(err?.response?.data?.message || 'Failed to load crises.', 'error');
    } finally {
      this.loading = false;
    }
  }

  selectCrisis(crisis: Crisis): void {
    this.selectedCrisisId = crisis.crisisId;
    this.type = this.typeOptions.find(o => o.label === crisis.type)?.value ?? 0;
    this.location = crisis.location;
    this.severity = this.severityOptions.find(o => o.label === crisis.severity)?.value ?? 0;
    const statusIndex = this.crisisStatusOptions.indexOf(crisis.status);
    this.status = statusIndex >= 0 ? statusIndex : 0;
  }

  clearSelection(): void { this.selectedCrisisId = null; this.resetForm(); }
  resetForm(): void { this.type = 0; this.location = ''; this.severity = 0; this.status = 0; }

  async saveCrisis(): Promise<void> {
    if (!this.canCreateOrUpdate || !this.location.trim()) return;
    this.submitting = true;

    try {
      if (this.selectedCrisisId) {
        await this.api.updateCrisis(this.selectedCrisisId, this.status, this.severity);
        this.showMessage('Crisis updated.', 'success');
      } else {
        const payload = {
          type: this.type,
          location: this.location.trim(),
          date: new Date().toISOString(),
          severity: this.severity,
          status: this.status,
        };
        await this.api.createCrisis(payload);
        this.showMessage('Crisis declared.', 'success');
      }
      this.clearSelection();
      await this.refresh();
    } catch (err: any) {
      this.showMessage(err?.response?.data?.message || 'Failed to save crisis.', 'error');
    } finally {
      this.submitting = false;
    }
  }

  statusClass(status: string): string {
    if (status === 'Active') return 'bg-emerald-50 text-emerald-700';
    if (status === 'Resolved' || status === 'Closed') return 'bg-slate-100 text-slate-600';
    if (status === 'Cancelled') return 'bg-red-50 text-red-700';
    return 'bg-amber-50 text-amber-700';
  }

  private showMessage(msg: string, type: 'success' | 'error'): void {
    this.message = msg;
    this.messageType = type;
  }
}
