import { Component, OnInit, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { EdraApiService, Dispatch } from '../../../core/services/api/edra-api.service';
import { Sidebar } from '../../../layout/sidebar/sidebar';
import { getUserRole, normalizeRole } from '../../../shared/auth-utils';

@Component({
  selector: 'app-edra-dispatch-page',
  standalone: true,
  imports: [FormsModule, Sidebar],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './edra-dispatch-page.html',
  styleUrl: './edra-dispatch-page.css',
})
export class EdraDispatchPage implements OnInit {
  dispatcherOnly = false;
  dispatches: Dispatch[] = [];
  loading = false;
  submitting = false;
  error = '';
  newResourceId = 0;
  newIncidentId = 0;
  pendingIncidentCount = 0;
  availableResourceCount = 0;
  get newDispatchResourceId() { return this.newResourceId; }
  set newDispatchResourceId(val: number) { this.newResourceId = val; }
  get newDispatchIncidentId() { return this.newIncidentId; }
  set newDispatchIncidentId(val: number) { this.newIncidentId = val; }

  readonly statusOptions = ['EnRoute', 'OnSite', 'Cancelled'];

  constructor(private api: EdraApiService) { }

  ngOnInit(): void {
    this.dispatcherOnly = normalizeRole(getUserRole()) === 'emergency_dispatcher';
    this.loadDashboard();
    this.loadDispatches();
  }

  async loadDashboard(): Promise<void> {
    try {
      const data = await this.api.getDashboard();
      this.pendingIncidentCount = data.incidentPending?.length ?? 0;
      this.availableResourceCount = data.resources?.length ?? 0;
    } catch { /* silent */ }
  }

  async loadDispatches(): Promise<void> {
    this.loading = true;
    this.error = '';
    try {
      this.dispatches = await this.api.getDispatches();
    } catch (err: any) {
      this.error = err?.response?.data?.message || 'Failed to load dispatches.';
    } finally {
      this.loading = false;
    }
  }

  async dispatchResource(): Promise<void> {
    if (!this.newResourceId || !this.newIncidentId) {
      alert('Resource ID and Incident ID required.');
      return;
    }
    this.submitting = true;

    const payload = { resourceID: this.newResourceId, incidentID: this.newIncidentId };

    try {
      await this.api.dispatchResource(payload);
      this.newResourceId = 0;
      this.newIncidentId = 0;
      await this.loadDashboard();
      await this.loadDispatches();
    } catch (err: any) {
      alert(err?.response?.data?.message || 'Failed to dispatch.');
    } finally {
      this.submitting = false;
    }
  }

  async assignResource(): Promise<void> {
    if (!this.newResourceId || !this.newIncidentId) {
      alert('Resource ID and Incident ID required.');
      return;
    }
    this.submitting = true;

    const payload = { resourceID: this.newResourceId, incidentID: this.newIncidentId };

    try {
      await this.api.assignResource(payload);
      this.newResourceId = 0;
      this.newIncidentId = 0;
      await this.loadDashboard();
      await this.loadDispatches();
    } catch (err: any) {
      alert(err?.response?.data?.message || 'Failed to assign.');
    } finally {
      this.submitting = false;
    }
  }

  async updateDispatchStatus(dispatchId: number, status: string): Promise<void> {
    const statusMap: Record<string, number> = { Assigned: 0, EnRoute: 1, OnSite: 2, Resolved: 3, Cancelled: 4 };
    const statusValue = statusMap[status];
    if (statusValue === undefined) return;

    this.submitting = true;

    const payload = { status: statusValue };

    try {
      await this.api.updateDispatchStatus(dispatchId, payload);
      await this.loadDispatches();
      await this.loadDashboard();
    } catch (err: any) {
      alert(err?.response?.data?.message || 'Failed to update status.');
    } finally {
      this.submitting = false;
    }
  }

  async completeDispatch(event: any): Promise<void> {
    const dispatchId = event?.dispatchId || event as number;
    this.submitting = true;
    try {
      await this.api.completeDispatch(dispatchId);
      await this.loadDashboard();
      await this.loadDispatches();
    } catch (err: any) {
      alert(err?.response?.data?.message || 'Failed to complete dispatch.');
    } finally {
      this.submitting = false;
    }
  }

  updateDispatchStatusEvent(event: any): void {
    this.updateDispatchStatus(event.dispatchId, event.status);
  }
}
